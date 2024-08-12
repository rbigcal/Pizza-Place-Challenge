

using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pizza_Place_Challenge.API.Base.Models;
using Pizza_Place_Challenge.API.CSV.Models;
using Pizza_Place_Challenge.Core.Data;
using Pizza_Place_Challenge.Core.Data.Entities;
using Pizza_Place_Challenge.Core.Enumerations;
using System.Globalization;
using System.Net;
using static Pizza_Place_Challenge.API.PizzaTypesController;

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
            [JsonProperty(PropertyName = "pizza")]
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
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
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
        public async Task<NewEditPizzaModel> NewPizzaAsync(string pizzatype_code, PizzaSizes_Enumeration size, float price)
        {
            NewEditPizzaModel result = new();

            try
            {
                PizzaRepository repository = new PizzaRepository(_context);
                string pizzaid = string.Empty;
                if (string.IsNullOrEmpty(pizzatype_code)) {
                    result.SetStatus(HttpStatusCode.BadRequest, "Pizza type code is required");
                    return result;
                }

                PizzaType pizzatype = await new PizzaTypeRepository(_context).GetPizzaTypeByCode(pizzatype_code);

                if (pizzatype == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza type not found");
                    return result;
                }

                pizzaid = $"{pizzatype_code}_{size.ToString()}";

                Pizza pizza = new Pizza()
                {
                    PizzaId = pizzaid,
                    ID_PizzaType = pizzatype.Id,
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

        [HttpPut, Route("action/upload-csv"), AllowAnonymous]
        public async Task<AllPizzasModel> NewPizzaTypeUploadCSVAsync(IFormFile pizzatype_csv) {
            AllPizzasModel result = new();

            try {

                PizzaRepository repository = new PizzaRepository(_context);

                List<Pizza> newpizza_list = new();
                List<CSV_Pizza> csv_pizzalist = new();

                if (pizzatype_csv == null || pizzatype_csv.Length == 0) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Cannot read csv file");
                }

                if (pizzatype_csv != null) {
                    using (var stream = pizzatype_csv.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture))) {
                        csv_pizzalist = csv.GetRecords<CSV_Pizza>().ToList();
                    }
                }

                List<PizzaType> pizzatype_list = await new PizzaTypeRepository(_context).GetAllAsync();

                foreach (CSV_Pizza pizza in csv_pizzalist) {

                    PizzaSizes_Enumeration size = PizzaSizes_Enumeration.S;

                    switch (pizza.Size.ToLower()) {
                        case "s":
                            size = PizzaSizes_Enumeration.S;
                            break;
                        case "m":
                            size = PizzaSizes_Enumeration.M;
                            break;
                        case "l":
                            size = PizzaSizes_Enumeration.L;
                            break;
                        case "xl":
                            size = PizzaSizes_Enumeration.XL;
                            break;
                        case "xxl":
                            size = PizzaSizes_Enumeration.XXL;
                            break;
                    }

                    string id_pizzatype = string.Empty;

                    if (!string.IsNullOrEmpty(pizza.PizzaTypeId)) {
                        if (pizzatype_list.Any()) {
                            PizzaType pizzatype = pizzatype_list.FirstOrDefault(i => i.Code ==  pizza.PizzaTypeId);

                            if(pizzatype != null) {
                                id_pizzatype = pizzatype.Id;
                            }
                        }
                    }

                    float pizzaprice = 0;
                    float.TryParse(pizza.Price, out pizzaprice);

                    Pizza new_pizza = new Pizza() {
                        ID_PizzaType = id_pizzatype,
                        PizzaId = pizza.PizzaId,
                        Price = pizzaprice,
                        Size = size
                    };

                    newpizza_list.Add(new_pizza);
                }

                await repository.AddAsync(newpizza_list);
                result.Pizzas = newpizza_list;
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPatch, Route("action/edit"), AllowAnonymous]
        public async Task<NewEditPizzaModel> EditPizzaAsync(string id, string pizzatype_code, PizzaSizes_Enumeration size, float price)
        {
            NewEditPizzaModel result = new();

            try
            {
                PizzaRepository repository = new PizzaRepository(_context);

                Pizza pizza = await repository.GetByIdAsync(id);
                string pizzaid = string.Empty;

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
                    return result;
                }

                PizzaType pizzatype = await new PizzaTypeRepository(_context).GetPizzaTypeByCode(pizzatype_code);

                if (pizzatype == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza type not found");
                    return result;
                }

                pizzaid = $"{pizzatype_code}_{size.ToString()}";

                pizza.ID_PizzaType = pizzatype.Id;
                pizza.PizzaId = pizzaid;
                pizza.Price = price;
                pizza.Size = size;

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
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
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

