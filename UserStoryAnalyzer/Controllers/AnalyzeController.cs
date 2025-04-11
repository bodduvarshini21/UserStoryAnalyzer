using Microsoft.AspNetCore.Mvc;
using UserStoryAnalyzer.Models;
using CsvHelper;
using System.Globalization;

namespace OpenAI_ChatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatCompletionController(IChatCompletionService chatCompletionService) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var stories = new List<UserStory>();
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            stories = csv.GetRecords<UserStory>().ToList();

            return Ok(await chatCompletionService.GetChatCompletionAsync(stories));
        }
    }
}