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
	public class PersonasController : Controller
	{
		private ProyectoIEntities1 db = new ProyectoIEntities1();

		// GET: Personas
		public ActionResult Index()
		{
			return View(db.Personas.ToList());
		}




		// GET: Personas/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Personas personas = db.Personas.Find(id);
			if (personas == null)
			{
				return HttpNotFound();
			}
			return View(personas);
		}

		// GET: Personas/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Personas/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,CedulaIdentidad,Nombre,Apellidos,FechaRegistro,Estado")] Personas personas)
		{
			/*  if (ModelState.IsValid)
              {
                  db.Personas.Add(personas);
                  //db.InsertarSocio();
                  db.SaveChanges();
                  return RedirectToAction("Index");
              }

           
          }*/
			if (ModelState.IsValid)
			{

			

				try
				{
					// Llamar al procedimiento almacenado para insertar el préstamo
					db.Database.ExecuteSqlCommand(
					   "EXEC InsertarSocio @CedulaIdentidad, @Nombre, @Apellidos, @FechaRegistro,@Estado",
				   new SqlParameter("@CedulaIdentidad", personas.CedulaIdentidad),
					new SqlParameter("@Nombre", personas.Nombre),
					new SqlParameter("@Apellidos", personas.Apellidos),
					new SqlParameter("@FechaRegistro", DateTime.Now),
					new SqlParameter("@Estado", personas.Estado)
					);

				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Error al guardar el registro: " + ex.Message);
					//ModelState.AddModelError("", "Error al guardar el registro:  La cédula de identidad ya existe.");
					return View(personas);
				}


				return RedirectToAction("Index", "Personas");
			}


			return View(personas);
		}

		// GET: Personas/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Personas personas = db.Personas.Find(id);
			if (personas == null)
			{
				return HttpNotFound();
			}
			return View(personas);
		}

		// POST: Personas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,CedulaIdentidad,Nombre,Apellidos,FechaRegistro,Estado")] Personas personas)
		{
			if (ModelState.IsValid)
			{
				db.Entry(personas).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(personas);
		}

		// GET: Personas/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Personas personas = db.Personas.Find(id);
			if (personas == null)
			{
				return HttpNotFound();
			}
			return View(personas);
		}

		// POST: Personas/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Personas personas = db.Personas.Find(id);
			db.Personas.Remove(personas);
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
