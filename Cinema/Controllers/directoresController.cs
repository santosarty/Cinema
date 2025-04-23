using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema;

namespace Cinema.Controllers
{
    public class directoresController : Controller
    {
        private cinemaEntities db = new cinemaEntities();

        public ActionResult Index(int pagina = 1, int registros = 6, string searchString = "")
        {
            var director = db.director.Include(a => a.nacionalidad);
            if (!string.IsNullOrEmpty(searchString))
            {
                director = director.Where(a => a.idDirector.Equals(searchString) ||
                                                  a.nombre.Contains(searchString) ||
                                                  a.genero.Contains(searchString) ||
                                                  a.fechaNacimiento.Equals(searchString) ||
                                                  a.nacionalidad.descripcion.Contains(searchString));
            }

            director = director.OrderBy(a => a.idDirector);

            var totalDirectores = director.Count();
            var directoresP = director.Skip((pagina - 1) * registros).Take(registros).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalDirectores / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;
            return View(directoresP);
        }

        public ActionResult Create()
        {
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idDirector,nombre,genero,idNacionalidad,fechaNacimiento")] director director)
        {

            if (db.director.Any())
            {
                director.idDirector = db.director.ToList().Last().idDirector + 1;
            }
            else { director.idDirector = 1; }

            if (ModelState.IsValid)
            {
                db.director.Add(director);
                db.SaveChanges();
                TempData["MsgRegistroExitoso"] = "El registro se realizó exitosamente.";
                return RedirectToAction("Create");
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos, director.genero);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", director.idNacionalidad);
            return View(director);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            director director = db.director.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", director.idNacionalidad);
            return View(director);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idDirector,nombre,genero,idNacionalidad,fechaNacimiento")] director director)
        {
            if (ModelState.IsValid)
            {
                db.Entry(director).State = EntityState.Modified;
                db.SaveChanges();
                TempData["MsgModificacionExitosa"] = "La modificación se realizó exitosamente.";
                return RedirectToAction("Index");
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos, director.genero);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", director.idNacionalidad);
            return View(director);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            director director = db.director.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            director director = db.director.Find(id);
            db.director.Remove(director);
            db.SaveChanges();
            TempData["MsgEliminacionExitosa"] = "La eliminación se realizó exitosamente.";
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
