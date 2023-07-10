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
    public class PrestamosController : Controller
    {
        private ProyectoIEntities1 db = new ProyectoIEntities1();

        // GET: Prestamos
        public ActionResult Index()
        {
            var prestamos = db.Prestamos.Include(p => p.Libros).Include(p => p.Personas);
            return View(prestamos.ToList());
        }




        // GET: prestamos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prestamos prestamos = db.Prestamos.Find(id);
            if (prestamos == null)
            {
                return HttpNotFound();
            }
            return View(prestamos);
        }

        // GET: Prestamos/Create
        public ActionResult Create()
        {
            var libros = db.Libros.ToList();
            var personas = db.Personas.ToList();

            var librosSelectList = libros.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.ISBN} - {l.Titulo}"
            });
            ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");

            var personasSelectList = personas.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.CedulaIdentidad} - {p.Nombre} {p.Apellidos}"
            });
            ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");

            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdPersona,IdLibro,FechaPrestamo,FechaDevolucion,Devuelto")] Prestamos prestamos)
        {
            var libros = db.Libros.ToList();
            var personas = db.Personas.ToList();
            var librosSelectList = libros.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.ISBN} - {l.Titulo}"
            });

            var personasSelectList = personas.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.CedulaIdentidad} - {p.Nombre} {p.Apellidos}"
            });

            // Obtener la persona seleccionada
            var personaSeleccionada = personas.FirstOrDefault(p => p.Id == prestamos.IdPersona);

            // Verificar si la persona existe y no tiene tres libros en préstamo
            if (personaSeleccionada == null)
            {
                ModelState.AddModelError("", "La persona seleccionada no existe");
            }
            else if (personaSeleccionada.Prestamos.Count(p => p.Devuelto == false) >= 3)
            {
                ModelState.AddModelError("", "La persona ya tiene el máximo de libros prestados");
            }

            // Validar la fecha de devolución
            if (prestamos.FechaDevolucion > DateTime.Now.AddMonths(1))
            {
                ModelState.AddModelError("", "La fecha de devolución no puede ser mayor a un mes a partir de la fecha de préstamo");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Llamar al procedimiento almacenado para insertar el préstamo
                    var fechaDevolucion = DateTime.Today.AddDays(7); // Obtener la fecha de devolución (7 días después del préstamo)
                    db.Database.ExecuteSqlCommand(
                        "EXEC PrestarLibro @IdPersona, @IdLibro, @FechaPrestamo, @FechaDevolucion",
                        new SqlParameter("@IdPersona", prestamos.IdPersona),
                        new SqlParameter("@IdLibro", prestamos.IdLibro),
                        new SqlParameter("@FechaPrestamo", DateTime.Now),
                        new SqlParameter("@FechaDevolucion", fechaDevolucion)
                    );

                    // Incrementar la cantidad disponible del libro
                    var libro = db.Libros.Find(prestamos.IdLibro);
                    libro.CantidadDisponible++;

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar el registro: " + ex.Message);

                    ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");
                    ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");

                    return View(prestamos);
                }
            }

            ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");
            ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");

            return View(prestamos);
        }
      
        // GET: Prestamos/Edit/5
        public ActionResult Edit(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prestamos prestamos = db.Prestamos.Find(id);
          
            if (prestamos == null)
            {
                return HttpNotFound();
            }

            var libros = db.Libros.ToList();
            var personas = db.Personas.ToList();

            var librosSelectList = libros.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.ISBN} - {l.Titulo}"
            });

            var personasSelectList = personas.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.CedulaIdentidad} - {p.Nombre} {p.Apellidos}"
            });
           
            ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");
            ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");

            // Asignar el valor seleccionado en las listas desplegables
            ViewBag.SelectedLibroId = prestamos.IdLibro;
            ViewBag.SelectedPersonaId = prestamos.IdPersona;

            return View(prestamos);
        }




        // POST: Prestamos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdPersona,IdLibro,FechaPrestamo,FechaDevolucion,Devuelto,FechaDevolucionReal")] Prestamos prestamos)
        {
            var libros = db.Libros.ToList();
            var personas = db.Personas.ToList();

            var librosSelectList = libros.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.ISBN} - {l.Titulo}"
            });

            var personasSelectList = personas.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.CedulaIdentidad} - {p.Nombre} {p.Apellidos}"
            });

            if (ModelState.IsValid)
            {
                try
                {
                    // Ejecutar el procedimiento almacenado para devolver el libro
                    db.Database.ExecuteSqlCommand(
                        "EXEC DevolverLibro @IdPrestamo, @FechaDevolucionReal",
                        new SqlParameter("@IdPrestamo", prestamos.Id),
                        new SqlParameter("@FechaDevolucionReal", prestamos.FechaDevolucionReal)
                    );  

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar el registro: " + ex.Message);

                    ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");
                    ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");

                    return View(prestamos);
                }
            }

            ViewBag.IdLibro = new SelectList(librosSelectList, "Value", "Text");
            ViewBag.IdPersona = new SelectList(personasSelectList, "Value", "Text");
            return View(prestamos);
        }

        // GET: Prestamos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prestamos prestamos = db.Prestamos.Find(id);
            if (prestamos == null)
            {
                return HttpNotFound();
            }
            return View(prestamos);
        }

        // POST: Prestamos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prestamos prestamos = db.Prestamos.Find(id);
            db.Prestamos.Remove(prestamos);

            // Reducir la cantidad disponible del libro
            var libro = db.Libros.Find(prestamos.IdLibro);
            //libro.CantidadDisponible--;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ObtenerCantidadLibrosDisponibles(int idLibro)
        {
            var cantidadLibrosDisponibles = db.Libros.Where(l => l.Id == idLibro).Select(l => l.CantidadDisponible).FirstOrDefault();
            return Content(cantidadLibrosDisponibles.ToString());
        }

        public ActionResult Filter(string isbn, string cedula)
        {
            var prestamos = db.Prestamos.Include(p => p.Libros).Include(p => p.Personas);

            if (!string.IsNullOrEmpty(isbn))
            {
                prestamos = prestamos.Where(p => p.Libros.ISBN.Contains(isbn));
            }

            if (!string.IsNullOrEmpty(cedula))
            {
                prestamos = prestamos.Where(p => p.Personas.CedulaIdentidad.Contains(cedula));
            }

            return PartialView("_PrestamosTable", prestamos.ToList());
        }



        // GET: Prestamos/Devolver/5
        public ActionResult Devolver(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Prestamos prestamo = db.Prestamos.Find(id);

            if (prestamo == null)
            {
                return HttpNotFound();
            }

            return View(prestamo);
        }

        // POST: Prestamos/Devolver/5
        [HttpPost, ActionName("Devolver")]
        [ValidateAntiForgeryToken]
        public ActionResult DevolverConfirmed(int id)
        {
            Prestamos prestamo = db.Prestamos.Find(id);

            if (prestamo == null)
            {
                return HttpNotFound();
            }

            prestamo.Devuelto = true;
            prestamo.FechaDevolucionReal = DateTime.Now;

            db.Entry(prestamo).State = EntityState.Modified;
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
