using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Practice_project1.Dto;
using Practice_project1.Models;
using Practice_project1.Service;
using System.Security.Claims;

namespace Practice_project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;

        public PostController(UserService userService, PostService postService)
        {
            _userService = userService;
            _postService = postService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost(CreatePostDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if( userId == null)
            {
                return Unauthorized("Unauthorized");
            }
            var newPost = new Post {
                title = dto.title,
                description = dto.description, 
                userId = userId
            };

            //var currUser = await _userService.getUserByUserIDAsync(userId);
            //if (currUser.Posts == null)
            //    currUser.Posts = new List<Post>();

            //currUser.Posts.Add(newPost);

            //await _userService.updateUserAsync(currUser);
            await _postService.CreateAsyncPost(newPost);
            return Ok("Post Created Success Fully");

        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllPost()
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Console.Write(userId);
            //if(userId == null)
            //{
            //    return Unauthorized("User unauthorized");
            //}

            var posts = await _postService.GetAllPostAsync();
            return Ok(new { posts });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetPostById(string id)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Console.Write(userId);
            //if (userId == null)
            //{
            //    return Unauthorized("User unauthorized");
            //}
            var post = await _postService.GetPostById(id);
            return Ok(new { post });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePost(string id, UpdatePostDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User Unauthorized");
            }

            var updatedPost = new Post
            {
                Id = id,
                title = dto.title,
                description = dto.description,
                userId = userId,
            };

            await _postService.UpdatePostAsync(id, updatedPost);

            return Ok("Post updated successfully");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult>  DeletePost(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User Unauthorized");
            }

            await _postService.DeletePostAsync(id);

            return Ok("Post deleted successfully");
        }

        [HttpGet("get-comments/{id}")]
        public async Task<IActionResult> GetComments(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (userId == null)
            //{
            //    return Unauthorized("User Unauthorized");
            //}

            var comments = await _postService.GetAllComments(id);
            //if (!comments)
            //{
            //    return NotFound("No comments found !");
            //}
            return Ok(new { comments });
        }
    }
}
