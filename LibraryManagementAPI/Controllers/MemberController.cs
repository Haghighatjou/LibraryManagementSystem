using LibraryManagement.Core.Models;
using LibraryManagement.Core.Services;
using LibraryManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController:ControllerBase
    {
        private readonly LibraryService libraryService;
        public MemberController(LibraryService libraryService)
        {
            this.libraryService = libraryService;
        }

        [Authorize]
        [HttpGet("Members")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await libraryService.GetMembersAsync();
            var membersDto = members.Select(m => new MemberDTO
            {
                Id = m.Id,
                MemberName = m.MemberName,
                PhoneNumber = m.PhoneNumber,
                IsActive = m.IsActive
            }).ToList();

            return Ok(membersDto);
        }

        [Authorize]
        [HttpPatch("{MemberId}")]
        public async Task<IActionResult> UpdateMember(int MemberId,UpdateMemberDTO dto)
        {

            var member = new Member
            {
                Id = MemberId,
                MemberName = dto.MemberName,
                PhoneNumber = dto.PhoneNumber
            };

            await libraryService.UpdateMemberAsync(member);

            return NoContent();
        }

        [Authorize]
        [HttpPatch("enableMember/{memberId}")]
        public async Task<IActionResult> EnableMember(int memberId)
        {
            await libraryService.EnableMemberAsync(memberId);

            return Ok("Member enabled successfully.");
        }

        [Authorize]
        [HttpPatch("disableMember/{memberId}")]
        public async Task<IActionResult> DisableMember(int memberId)
        {
            await libraryService.DisableMemberAsync(memberId);

            return Ok("Member disabled successfully.");
        }

        [Authorize]
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> FindMemberById(int memberId)
        {
            var member= await libraryService.FindMemberByIdAsync(memberId);

            MemberDTO memberDto = new MemberDTO
                {
                Id= member.Id,
                MemberName= member.MemberName,
                PhoneNumber= member.PhoneNumber,
                IsActive=member.IsActive
                };

            return Ok(memberDto);
        }

        [Authorize]
        [HttpPost("members")]
        public async Task<IActionResult> AddMember(CreateMemberDTO dto)
        {
            await libraryService.AddMemberAsync(dto.MemberName, dto.PhoneNumber);

            return NoContent();
        }
    }
}
