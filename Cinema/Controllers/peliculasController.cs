using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema;
using Cinema.Models;

namespace Cinema.Controllers
{
    public class peliculasController : Controller
    {
        private cinemaEntities db = new cinemaEntities();

        public ActionResult Index(int pagina = 1, int registros = 6, string searchString = "")
        {
            var pelicula = db.pelicula.Include(p => p.director).Include(p => p.estudio).Include(p => p.actor).Include(p => p.genero);
            if (!string.IsNullOrEmpty(searchString))
            {
                pelicula = pelicula.Where(a => a.idPelicula.Equals(searchString) ||
                                                 a.titulo.Contains(searchString) ||
                                                 a.fechaEstreno.Equals(searchString) ||
                                                 a.director.nombre.Contains(searchString) ||
                                                 a.duracion.Equals(searchString) ||
                                                 a.idioma.Contains(searchString) ||
                                                 a.presupuesto.Equals(searchString) ||
                                                 a.estudio.Equals(searchString));
            }

            pelicula = pelicula.OrderBy(a => a.idPelicula);

            var totalPeliculas = pelicula.Count();
            var peliculasP = pelicula.Skip((pagina - 1) * registros).Take(registros).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalPeliculas / registros);
            ViewBag.RegistrosPorPagina = registros;
            ViewBag.SearchString = searchString;

            return View(peliculasP);
        }

        public ActionResult Create()
        {
            ViewBag.idDirector = new SelectList(db.director, "idDirector", "nombre");
            ViewBag.idEstudio = new SelectList(db.estudio, "idEstudio", "nombre");            
            ViewBag.actor = new MultiSelectList(db.actor, "idActor", "nombre");
            ViewBag.genero = new MultiSelectList(db.genero, "idGenero", "descripcion");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idPelicula,titulo,fechaEstreno,idDirector,duracion,idioma,presupuesto,idEstudio,repartoIds,generosIds")] pelicula pelicula)
        {
            if(db.pelicula.Any())
            {
                pelicula.idPelicula = db.pelicula.ToList().Last().idPelicula + 1;
            } else { pelicula.idPelicula = 1; }
            foreach (int a in pelicula.repartoIds) {                
                pelicula.actor.Add(db.actor.Find(a));
            }
            foreach (int g in pelicula.generosIds) {
                pelicula.genero.Add(db.genero.Find(g));
            }

            if (ModelState.IsValid)
            {
                db.pelicula.Add(pelicula);                
                db.SaveChanges();
                TempData["MsgRegistroExitoso"] = "El registro se realizó exitosamente.";
                return RedirectToAction("Create");
            }
            ViewBag.idDirector = new SelectList(db.director, "idDirector", "nombre", pelicula.idDirector);
            ViewBag.idEstudio = new SelectList(db.estudio, "idEstudio", "nombre", pelicula.idEstudio);
            ViewBag.actor = new MultiSelectList(db.actor, "idActor", "nombre", pelicula.repartoIds);
            ViewBag.genero = new MultiSelectList(db.genero, "idGenero", "descripcion", pelicula.generosIds);
            return View(pelicula);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pelicula pelicula = db.pelicula.Find(id);
            if (pelicula == null)
            {
                return HttpNotFound();
            }
            ViewBag.idDirector = new SelectList(db.director, "idDirector", "nombre", pelicula.idDirector);
            ViewBag.idEstudio = new SelectList(db.estudio, "idEstudio", "nombre", pelicula.idEstudio);
            ViewBag.actor = new MultiSelectList(db.actor, "idActor", "nombre",pelicula.actor);
            ViewBag.genero = new MultiSelectList(db.genero, "idGenero", "descripcion",pelicula.genero);
            return View(pelicula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idPelicula,titulo,fechaEstreno,idDirector,duracion,idioma,presupuesto,idEstudio,repartoIds,generosIds")] pelicula pelicula)
        { 
            foreach (int a in pelicula.repartoIds)
            {
                pelicula.actor.Add(db.actor.Find(a));
            }
            foreach (int g in pelicula.generosIds)
            {
                pelicula.genero.Add(db.genero.Find(g));
            }
            if (ModelState.IsValid)
            {
                db.Entry(pelicula).State = EntityState.Modified;
                db.SaveChanges();
                TempData["MsgModificacionExitosa"] = "La modificación se realizó exitosamente.";
                return RedirectToAction("Index");
            }
            ViewBag.idDirector = new SelectList(db.director, "idDirector", "nombre", pelicula.idDirector);
            ViewBag.idEstudio = new SelectList(db.estudio, "idEstudio", "nombre", pelicula.idEstudio);
            ViewBag.actor = new MultiSelectList(db.actor, "idActor", "nombre", pelicula.repartoIds);
            ViewBag.genero = new MultiSelectList(db.genero, "idGenero", "descripcion", pelicula.generosIds);
            return View(pelicula);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pelicula pelicula = db.pelicula.Find(id);
            if (pelicula == null)
            {
                return HttpNotFound();
            }
            return View(pelicula);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            pelicula pelicula = db.pelicula.Find(id);
            db.pelicula.Remove(pelicula);
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
