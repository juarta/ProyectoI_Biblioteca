using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoI_Libreria.Models;

namespace ProyectoI_Libreria.Controllers
{
    public class LibrosController : Controller
    {
        private ProyectoIEntities1 db = new ProyectoIEntities1();

        // GET: Libros
        public ActionResult Index()
        {
            return View(db.Libros.ToList());
        }

        // GET: Libros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libros libros = db.Libros.Find(id);
            if (libros == null)
            {
                return HttpNotFound();
            }
            return View(libros);
        }

        // GET: Libros/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ISBN,Titulo,CasaEditorial,NumeroEdicion,Autor,CantidadDisponible")] Libros libros)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Libros.Add(libros);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}


            		if (ModelState.IsValid)
			{

			

				try
				{
					// Llamar al procedimiento almacenado para insertar el libro
					db.Database.ExecuteSqlCommand(
                       "EXEC InsertarLibro @ISBN, @Titulo, @CasaEditorial, @NumeroEdicion,@Autor, @CantidadDisponible ",
                   new SqlParameter("@ISBN", libros.ISBN),
                   new SqlParameter("@Titulo", libros.Titulo),
                    new SqlParameter("@CasaEditorial", libros.CasaEditorial),
					new SqlParameter("@NumeroEdicion", libros.NumeroEdicion),
					new SqlParameter("@Autor", libros.NumeroEdicion),
					new SqlParameter("@CantidadDisponible", libros.CantidadDisponible)
					);

				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Error al guardar el registro: " + ex.Message);
				
					return View(libros);
				}


                return RedirectToAction("Index");
            }


            return View(libros);
        }

        // GET: Libros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libros libros = db.Libros.Find(id);
            if (libros == null)
            {
                return HttpNotFound();
            }
            return View(libros);
        }

        // POST: Libros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ISBN,Titulo,CasaEditorial,NumeroEdicion,Autor,CantidadDisponible")] Libros libros)
        {
            if (ModelState.IsValid)
            {
                db.Entry(libros).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(libros);
        }

        // GET: Libros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Libros libros = db.Libros.Find(id);
            if (libros == null)
            {
                return HttpNotFound();
            }
            return View(libros);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Libros libros = db.Libros.Find(id);
            db.Libros.Remove(libros);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
