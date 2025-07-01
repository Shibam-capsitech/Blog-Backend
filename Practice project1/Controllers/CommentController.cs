using Microsoft.AspNetCore.Mvc;
using Practice_project1.Dto;
using Practice_project1.Models;
using Practice_project1.Service;
using System.Security.Claims;

namespace Practice_project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly CommentService _commentservice;

        public CommentController(CommentService commentservice) {
            _commentservice = commentservice;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("Unauthorized");
            }

            var newComment = new CommentModel
            {
                Content = dto.content,
                postId = dto.postId,
                userId = userId,
            };

            await _commentservice.CreateComment(newComment);
            return Ok(new{ newComment });
        }

        //[HttpGet("/get-all-post-comment/{postId}")]
        //public async Task<IActionResult> GetAllCommentsByPostId(string postId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId == null)
        //    {
        //        return Unauthorized("Unauthorized");
        //    }
        //    var comments = await _commentservice.GetAllCommentsByPostId(postId);
        //    return Ok( new{ comments});
        //}

        [HttpDelete("delete/{id}")]
        public async Task DeleteComment(string id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user == null)
            {
                Unauthorized("Unauthorized Access");

            }
            await _commentservice.DeleteComment(id);
            
        }
    }
}
