﻿using API.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class ProduitController : ControllerBase
    {
        private readonly ILogger<ProduitController> _logger;
        private ApiDbContext db;

        public ProduitController(ILogger<ProduitController> logger, ApiDbContext dbContext)
        {
            _logger = logger;
            db = dbContext;
        }

        // GET ALL
        [HttpGet]
        public ActionResult<IEnumerable<Produit>> Get()
        {
            return Ok(
                db.Produits
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .Include(p => p.Purchaser)
            );
        }

        // GET BY ID
        [HttpGet("{id}")]
        public ActionResult<Produit> GetById(int id)
        {
            var produit = db.Produits
                                    .Include(p => p.Category)
                                    .Include(p => p.Seller)
                                    .Include(p => p.Purchaser)
                                    .Where(e => e.Id == id)
                                    .FirstOrDefault();

            if (produit != null)
                return Ok(produit);

            return NotFound();
        }

        // GET BY CATEGORY
        [HttpGet]
        [Route("Category/{id:int}")]
        public ActionResult<IEnumerable<Produit>> GetByCategoryId(int id)
        {
            var produits = db.Produits
                                    .Include(p => p.Category)
                                    .Include(p => p.Seller)
                                    .Include(p => p.Purchaser)
                                    .Where(e => e.Category.Id == id)
                                    .ToList();

            if (produits != null)
                return Ok(produits);

            return NotFound();
        }

        // POST
        [HttpPost]
        [Route("New")]
        public IActionResult Post(Produit produit)
        {
            try
            {
                var category = db.Categories.Find(produit.CategoryId);
                var seller = db.Users.Find(produit.SellerId);
                var purchaser = db.Users.Find(produit.PurchaserId);

                produit.Category = category;
                produit.Seller = seller;
                produit.Purchaser = purchaser;

                db.Produits.Add(produit);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // PUT
        [HttpPut]
        [Route("Edit/{id:int}")]
        public IActionResult Put(int id, Produit produitPut)
        {
            try
            {
                if (id != produitPut.Id)
                    return BadRequest();

                var produit = db.Produits.Find(id);

                if (produit == null)
                    return NotFound();

                produit.Libelle = produitPut.Libelle;
                produit.Description = produitPut.Description;
                produit.Price = produitPut.Price;
                produit.Quantity = produitPut.Quantity;
                produit.Img = produitPut.Img;
                produit.DatePublish = produitPut.DatePublish;
                produit.SoldAt = produitPut.SoldAt;
                produit.Sold = produitPut.Sold;
                produit.SellerId = produitPut.SellerId;
                produit.PurchaserId = produitPut.PurchaserId;
                produit.CategoryId = produitPut.CategoryId;

                var category = db.Categories.Find(produit.CategoryId);
                var seller = db.Users.Find(produit.SellerId);
                var purchaser = db.Users.Find(produit.PurchaserId);

                produit.Category = category;
                produit.Seller = seller;
                produit.Purchaser = purchaser;

                db.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE
        [HttpDelete]
        [Route("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var produit = db.Produits.Find(id);

            if (produit == null)
                return BadRequest();

            db.Produits.Remove(produit);
            db.SaveChanges();

            return Ok($"Produit with id \"{id}\" has been removed !");
        }
    }
}
