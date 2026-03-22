using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        public TradesController(ITradeService tradeService)  
        {
            // add interfaces and DI in this
            _tradeService = tradeService;
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<object>> AddTrades([FromBody] TradesDto inputTrade)
        {
            if (inputTrade == null) {

                return BadRequest("Data is emptys");
            }

            if (IsNullOrEmpty(inputTrade.Symbol) || IsNullOrEmpty(inputTrade.InstrumentType) || IsNullOrEmpty(inputTrade.TradeType))
            {
                return BadRequest("Data is invalid");
            }

            var result = await _tradeService.ValidateTrades(inputTrade);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);

             //if (inputTrade.)

            //if ()
            //{
                // return data null with valid reponse 
            //}
            // take data from the body and use service to save the data

            // proper response with status code for saving the data
        }

        [HttpGet]
        [Route("getdata")]
        public async Task<ActionResult<object>> GetData ()
        {
            var result = await _tradeService.GetTrades();

            if (!result.Success) {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch]
        [Route("edit/{id}")]
        public async Task<ActionResult<object>> EditData(string id, [FromBody] UpdateTradeDto tradeData)
        {
            if (id == null)
            {
                return BadRequest("Id can not be null");
            }

            if (tradeData == null)
            {
                return BadRequest("Data can't be null");
            }

            var result = await _tradeService.EditTrade(Guid.Parse(id), tradeData);

            if (!result.Success) {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("deleteall")]
        public async Task<ActionResult<object>> DeleteAllTrades()
        {
            var result = await _tradeService.DeleteAllTrades();

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult<object>> DeleteTrade(Guid id)
        {

            if (id == Guid.Empty)
            {
                return BadRequest(new Response { Success = false, Message = "Please give a valid id", Data = null });
            }
            var result = await _tradeService.DeleteTrade(id);

            if (!result.Success) {
                return BadRequest(result);
            }

            return result;
        }


        private static bool IsNullOrEmpty(string value) 
        {
            if (String.IsNullOrEmpty(value))
            {
                return true;
            }

            return false;
        }

    }
}
