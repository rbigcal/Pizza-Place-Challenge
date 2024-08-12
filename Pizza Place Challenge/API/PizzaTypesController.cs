

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

namespace Pizza_Place_Challenge.API
{
    [Route("api/[controller]")]
    [ApiController, AllowAnonymous]
    public class PizzaTypesController : ControllerBase
    {
        #region . Setup                .
        private DataContext _context { get; set; }

        public PizzaTypesController(DataContext context)
        {
            _context = context;
        }
        #endregion
        #region . Locals               .

        #endregion

        #region . API Endpoint Models  .

        public class AllPizzaTypesModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "pizzatypes")]
            public List<PizzaType> PizzaTypes { get; set; } = new();
        }

        public class ByPizzaTypeModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "pizzatype")]
            public PizzaType PizzaType { get; set; }
        }

        public class NewEditPizzaTypeModel : ApiControllerModel
        {
            [JsonProperty(PropertyName = "pizzatype")]
            public PizzaType PizzaType { get; set; }
        }

        #endregion
        #region . API Endpoints        .

        [HttpGet, Route("query/all"), AllowAnonymous]
        public async Task<AllPizzaTypesModel> GetAllPizzaTypesAsync()
        {
            AllPizzaTypesModel result = new();

            try
            {
                PizzaTypeRepository repository = new PizzaTypeRepository(_context);
                result.PizzaTypes = await repository.GetAllAsync();
            }
            catch (Exception ex) 
            { 
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/by-id"), AllowAnonymous]
        public async Task<ByPizzaTypeModel> ByPizzaTypeIdAsync(
            string id
        )
        {
            ByPizzaTypeModel result = new();

            try {
                PizzaTypeRepository repository = new PizzaTypeRepository(_context);
                PizzaType pizzatype = await repository.GetByIdAsync(id);

                if (pizzatype == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza Type not found");
                    return result;
                }

                result.PizzaType = pizzatype;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new"), AllowAnonymous]
        public async Task<NewEditPizzaTypeModel> NewPizzaTypeAsync(PizzaCategories_Enumeration category, string name, string code, string ingredients)
        {
            NewEditPizzaTypeModel result = new();

            try
            {

                PizzaTypeRepository repository = new PizzaTypeRepository(_context);

                PizzaType pizzatype = new PizzaType()
                {
                    Name = name,
                    Code = code,
                    Category = category,
                    Ingredients = ingredients
                };

                await repository.AddAsync(pizzatype);
                result.PizzaType = pizzatype;
            }
            catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/upload-csv"), AllowAnonymous]
        public async Task<AllPizzaTypesModel> NewPizzaTypeUploadCSVAsync(IFormFile pizzatype_csv) {
            AllPizzaTypesModel result = new();

            try {

                PizzaTypeRepository repository = new PizzaTypeRepository(_context);

                List<PizzaType> newpizzatypes = new();
                List<CSV_PizzaType> pizzatypes = new();

                if (pizzatype_csv == null || pizzatype_csv.Length == 0) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Cannot read csv file");
                }

                if (pizzatype_csv != null) {
                    using (var stream = pizzatype_csv.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture))) {
                        pizzatypes = csv.GetRecords<CSV_PizzaType>().ToList();
                    }
                }

                foreach (CSV_PizzaType pizzatype in pizzatypes) {

                    PizzaCategories_Enumeration category = PizzaCategories_Enumeration.Classic;
                    
                    switch(pizzatype.Category.ToLower()) {
                        case "chicken":
                            category = PizzaCategories_Enumeration.Chicken;
                            break;
                        case "classic":
                            category = PizzaCategories_Enumeration.Classic;
                            break;
                        case "supreme":
                            category = PizzaCategories_Enumeration.Supreme;
                            break;
                        case "veggie":
                            category = PizzaCategories_Enumeration.Veggie;
                            break;
                    }

                    PizzaType new_pizzatype = new PizzaType() {
                        Name = pizzatype.Name,
                        Code = pizzatype.PizzaTypeId,
                        Ingredients = pizzatype.Ingredients,
                        Category = category
                    };

                    await repository.AddAsync(new_pizzatype);
                    newpizzatypes.Add(new_pizzatype);
                }

                result.PizzaTypes = newpizzatypes;
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPatch, Route("action/edit"), AllowAnonymous]
        public async Task<NewEditPizzaTypeModel> EditPizzaTypeAsync(string id, PizzaCategories_Enumeration category, string name, string code, string ingredients)
        {
            NewEditPizzaTypeModel result = new();

            try
            {
                PizzaTypeRepository repository = new PizzaTypeRepository(_context);

                PizzaType pizzatype = await repository.GetByIdAsync(id);

                if (pizzatype == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza Type not found");
                    return result;
                }

                pizzatype.Name = name;
                pizzatype.Code = code;
                pizzatype.Category = category;
                pizzatype.Ingredients = ingredients;

                await repository.UpdateAsync(pizzatype);
                result.PizzaType = pizzatype;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpDelete, Route("action/delete"), AllowAnonymous]
        public async Task<ApiControllerModel> DeletePizzaTypeAsync(string id)
        {
            ApiControllerModel result = new();

            try
            {
                PizzaTypeRepository repository = new PizzaTypeRepository(_context);

                PizzaType pizza = await repository.GetByIdAsync(id);

                if (pizza == null)
                {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza Type not found");
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

