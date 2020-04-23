using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

//Endpoint => URL
//http://localhost:5000
//https://localhost:5001

[Route("categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] - Inativar Cache em caso de caching global
    public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Post([FromBody]Category model, [FromServices]DataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();

            return Ok(model);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível criar a categoria." });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> PUT(
        int id,
        [FromBody]Category model,
        [FromServices]DataContext context)
    {
        if (id != model.Id)
            return NotFound(new { message = "Categoria não encontrada" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria." });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices]DataContext context)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada." });
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso." });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível remover a categoria." });
        }
    }
}