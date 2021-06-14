using System;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class BuggyController : BaseApiController
    {
        public DataContext _context { get; set; }
        public BuggyController(DataContext context){
            _context = context;
        }
        // 401 Unauthorized
        // Erro: Autorização necessária
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }

        // 404 Not Found
        // Se não passar um texto como parametro um objeto é retornado descrevendo o erro
        [HttpGet("not-found")]
        public ActionResult<UserModel> GetNotFound(){
            var thing =  _context.Users.Find(-1);

            // if (thing == null) return NotFound("This is a not found request!");
            if (thing == null) return NotFound();
            return Ok(thing);
        }

        // 500 Internal Server Error
        // Erro: Aplicar ToString em null
        // Esse é o único que da para tratar com try-catch
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError(){
            var thing =  _context.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }

        [HttpGet("server-error-try")]
        public ActionResult<string> GetServerErrorTry(){
            var thing =  _context.Users.Find(-1);

            try{
                var thingToReturn = thing.ToString();
                return thingToReturn;
            }
            catch(Exception ex){
                return StatusCode(503,"Computer says no!");
            }
        }

        // 400 Bad Request
        // Se não passar um texto como parametro um objeto é retornado descrevendo o erro
        [HttpGet("bad-request")]
        public ActionResult<UserModel> GetBadRequest(){
            // return BadRequest("This is a bad request!");
            return BadRequest();
        }
    }
}