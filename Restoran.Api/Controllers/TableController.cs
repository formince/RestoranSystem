using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Core.Business;
using Restoran.Core.DTOs.Table;

namespace Restoran.Api.Controllers
{
    public class TableController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTables()
        {
            var tableBLL = new BLLTable();
            var tables = await tableBLL.GetTablesAsync();
            return HandleResult(tables, "Masalar başarıyla getirildi");
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTable(int id)
        {
            var tableBLL = new BLLTable();
            var table = await tableBLL.GetTableByIdAsync(id);
            return HandleResult(table, "Masa başarıyla getirildi");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateTable(TableCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tableBLL = new BLLTable();
            var result = await tableBLL.CreateTableAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateTable(int id, TableUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tableBLL = new BLLTable();
            var result = await tableBLL.UpdateTableAsync(id, dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var tableBLL = new BLLTable();
            var result = await tableBLL.DeleteTableAsync(id);
            return HandleResult(result);
        }
    }
} 