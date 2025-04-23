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
    public class actoresController : Controller
    {
        private cinemaEntities db = new cinemaEntities();

        public ActionResult Index(int pagina = 1, int registros = 6, string searchString = "")
        {
            var actor = db.actor.Include(a => a.nacionalidad);
            if (!string.IsNullOrEmpty(searchString))
            {
                actor = actor.Where(a => a.idActor.Equals(searchString) ||
                                                 a.nombre.Contains(searchString) ||
                                                 a.genero.Contains(searchString) ||
                                                 a.fechaNacimiento.Equals(searchString) ||
                                                 a.nacionalidad.descripcion.Contains(searchString));
            }

            actor = actor.OrderBy(a => a.idActor);

            var totalActores = actor.Count();
            var actoresP = actor.Skip((pagina - 1) * registros).Take(registros).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalActores / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;
            return View(actoresP);
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
        public ActionResult Create([Bind(Include = "idActor,nombre,genero,idNacionalidad,fechaNacimiento")] actor actor)
        {
            if (db.actor.Any())
            {
                actor.idActor = db.actor.ToList().Last().idActor + 1;
            }
            else { actor.idActor = 1; }

            if (ModelState.IsValid)
            {
                db.actor.Add(actor);
                db.SaveChanges();
                TempData["MsgRegistroExitoso"] = "El registro se realizó exitosamente.";
                return RedirectToAction("Create");
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos, actor.genero);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", actor.idNacionalidad);
            return View(actor);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            actor actor = db.actor.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", actor.idNacionalidad);
            return View(actor);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idActor,nombre,genero,idNacionalidad,fechaNacimiento")] actor actor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actor).State = EntityState.Modified;
                db.SaveChanges();
                TempData["MsgModificacionExitosa"] = "La modificación se realizó exitosamente.";
                return RedirectToAction("Index");
            }
            List<string> generos = new List<string> { "Selecciona un género", "Masculino", "Femenino", "No binario", "Fluido" };
            ViewBag.genero = new SelectList(generos, actor.genero);
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", actor.idNacionalidad);
            return View(actor);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            actor actor = db.actor.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            actor actor = db.actor.Find(id);
            db.actor.Remove(actor);
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
