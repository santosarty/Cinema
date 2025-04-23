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
    public class estudiosController : Controller
    {
        private cinemaEntities db = new cinemaEntities();

        public ActionResult Index(int pagina = 1, int registros = 6, string searchString = "")
        {
            var estudio = db.estudio.Include(a => a.nacionalidad);
            if (!string.IsNullOrEmpty(searchString))
            {
                estudio = estudio.Where(a => a.idEstudio.Equals(searchString) ||
                                                 a.nombre.Contains(searchString) ||
                                                 a.fundacion.Equals(searchString) ||
                                                 a.nacionalidad.descripcion.Contains(searchString));
            }

            estudio = estudio.OrderBy(a => a.idEstudio);

            var totalEstudios = estudio.Count();
            var estudiosP = estudio.Skip((pagina - 1) * registros).Take(registros).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalEstudios / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;
            return View(estudiosP);
        }

        public ActionResult Create()
        {
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEstudio,nombre,fundacion,idNacionalidad")] estudio estudio)
        {

            if (db.estudio.Any())
            {
                estudio.idEstudio = db.estudio.ToList().Last().idEstudio + 1;
            }
            else { estudio.idEstudio = 1; }

            if (ModelState.IsValid)
            {
                db.estudio.Add(estudio);
                db.SaveChanges();
                TempData["MsgRegistroExitoso"] = "El registro se realizó exitosamente.";
                return RedirectToAction("Create");
            }

            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", estudio.idNacionalidad);
            return View(estudio);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estudio estudio = db.estudio.Find(id);
            if (estudio == null)
            {
                return HttpNotFound();
            }
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", estudio.idNacionalidad);
            return View(estudio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEstudio,nombre,fundacion,idNacionalidad")] estudio estudio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estudio).State = EntityState.Modified;
                db.SaveChanges();
                TempData["MsgModificacionExitosa"] = "La modificación se realizó exitosamente.";
                return RedirectToAction("Index");
            }
            ViewBag.idNacionalidad = new SelectList(db.nacionalidad, "idNacionalidad", "descripcion", estudio.idNacionalidad);
            return View(estudio);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            estudio estudio = db.estudio.Find(id);
            if (estudio == null)
            {
                return HttpNotFound();
            }
            return View(estudio);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            estudio estudio = db.estudio.Find(id);
            db.estudio.Remove(estudio);
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
