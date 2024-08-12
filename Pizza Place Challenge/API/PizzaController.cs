

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pizza_Place_Challenge.API.Base.Models;
using Pizza_Place_Challenge.Core.Data;
using Pizza_Place_Challenge.Core.Data.Entities;
using Pizza_Place_Challenge.Core.Enumerations;
using System.Net;

namespace Pizza_Place_Challenge.API
{
    [Route("api/[controller]")]
    [ApiController, AllowAnonymous]
    public class PizzaController : ControllerBase
    {
        #region . Setup                .
        private DataContext _context { get; set; }

        public PizzaController(DataContext context)
        {
            _context = context;
        }
        #endregion
        #region . Locals               .

        #endregion

        #region . API Endpoint Models  .

        public class AllPizzasModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "pizzas")]
            public List<Pizza> Pizzas { get; set; } = new();
        }

        public class ByPizzaModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "pizza")]
            public Pizza Pizza { get; set; }
        }

        public class NewEditPizzaModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "device")]
            public Pizza Pizza { get; set; }
        }

        #endregion
        #region . API Endpoints        .

        [HttpGet, Route("query/all"), AllowAnonymous]
        public async Task<AllPizzasModel> GetAllPizzasAsync()
        {
            AllPizzasModel result = new();

            try
            {
                PizzaRepository repository = new PizzaRepository(_context);
                result.Pizzas = await repository.GetAllAsync();
            }
            catch (Exception ex) 
            { 
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/by-id"), AllowAnonymous]
        public async Task<ByPizzaModel> ByPizzaIdAsync(
            string id
        )
        {
            ByPizzaModel result = new();

            try {
                PizzaRepository repository = new PizzaRepository(_context);
                Pizza pizza = await repository.GetByIdAsync(id);

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Pizza not found");
                    return result;
                }

                result.Pizza = pizza;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new"), AllowAnonymous]
        public async Task<NewEditPizzaModel> NewPizzaAsync(string idpizzatype, PizzaSizes_Enumeration size, float price)
        {
            NewEditPizzaModel result = new();

            try
            {

                PizzaRepository repository = new PizzaRepository(_context);

                Pizza pizza = new Pizza()
                {
                    ID_PizzaType = idpizzatype,
                    Price = price,
                    Size = size
                };

                await repository.AddAsync(pizza);
                result.Pizza = pizza;
            }
            catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPatch, Route("action/edit"), AllowAnonymous]
        public async Task<NewEditPizzaModel> EditPizzaAsync(string id, string idpizzatype, PizzaSizes_Enumeration size, float price)
        {
            NewEditPizzaModel result = new();

            try
            {
                PizzaRepository repository = new PizzaRepository(_context);

                Pizza pizza = await repository.GetByIdAsync(id);

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Pizza not found");
                    return result;
                }

                await repository.UpdateAsync(pizza);
                result.Pizza = pizza;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpDelete, Route("action/delete"), AllowAnonymous]
        public async Task<ApiControllerModel> DeletePizzaAsync(string id)
        {
            ApiControllerModel result = new();

            try
            {
                PizzaRepository repository = new PizzaRepository(_context);

                Pizza pizza = await repository.GetByIdAsync(id);

                if (pizza == null)
                {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Pizza not found");
                    return result;
                }

                await repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }



        #endregion

        #region . Helper Methods       .

        #endregion
    }
}

