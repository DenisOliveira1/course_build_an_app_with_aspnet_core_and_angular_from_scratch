using api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{   
    // Já que as anotações estão aqui não precisa repetir elas em nenhuma classe que herde dessa classe
    
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}