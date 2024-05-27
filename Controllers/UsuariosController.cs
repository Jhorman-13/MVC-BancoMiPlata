using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiPlata.Models;
using Microsoft.AspNetCore.Http;


namespace MiPlata.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BancoContext _context;

        public UsuariosController(BancoContext context)
        {
            _context = context;
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Iniciar));
            }
            return View(usuario);
        }

        public IActionResult Iniciar()
        {
            return View();
        }

        [HttpPost]
        /* public async Task<IActionResult> Iniciar(string email, string contraseña)
         {
             var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
             if (usuario != null)
             {
                 if (usuario.IntentosRestantes > 0)
                 {
                     if (usuario.Contraseña == contraseña)
                     {
                         usuario.IntentosRestantes = 3;
                         await _context.SaveChangesAsync();

                         HttpContext.Session.SetInt32("ID_Usuario", usuario.IdUsuario);

                         return RedirectToAction(nameof(Menu), new { id = usuario.IdUsuario });
                     }
                     else
                     {
                         usuario.IntentosRestantes--;
                         await _context.SaveChangesAsync();
                         ModelState.AddModelError("", "Contraseña incorrecta. Intentos restantes: " + usuario.IntentosRestantes);
                     }
                 }
                 else
                 {
                     ModelState.AddModelError("", "Cuenta bloqueada. Por favor, contacte al administrador.");
                 }
             }
             else
             {
                 ModelState.AddModelError("", "Usuario no encontrado.");
             }

             return View();
         }*/


        [HttpPost]
        public async Task<IActionResult> Iniciar(string email, string contraseña)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (usuario != null)
            {
                if (usuario.IntentosRestantes > 0)
                {
                    if (usuario.Contraseña == contraseña)
                    {
                        usuario.IntentosRestantes = 3;
                        await _context.SaveChangesAsync();

                        HttpContext.Session.SetInt32("ID_Usuario", usuario.IdUsuario);

                        return RedirectToAction(nameof(Menu), new { id = usuario.IdUsuario });
                    }
                    else
                    {
                        usuario.IntentosRestantes--;
                        await _context.SaveChangesAsync();
                        ModelState.AddModelError("", "Contraseña incorrecta. Intentos restantes: " + usuario.IntentosRestantes);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Cuenta bloqueada. Por favor, contacte al administrador.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
            }

            return View();
        }


        // Consultar Saldo
        public IActionResult ConsultarSaldo(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            ViewBag.Saldo = usuario.Saldo;
            return View(usuario);
        }

        // Retirar
        public IActionResult Retirar(int id)
        {
            return View(new Transaccione { IdUsuario = id, Tipo = "Retiro" });
        }

        [HttpPost]
        public async Task<IActionResult> Retirar(Transaccione transaccion)
        {
            var usuario = await _context.Usuarios.FindAsync(transaccion.IdUsuario);
            if (transaccion.Monto > 0)
            {
                if (usuario.Saldo >= transaccion.Monto)
                {
                    usuario.Saldo -= transaccion.Monto;
                    transaccion.Fecha = DateTime.Now;
                    _context.Transacciones.Add(transaccion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Menu), new { id = usuario.IdUsuario });
                }
                ModelState.AddModelError("", "Saldo insuficiente");
            }
            else
            {
                ModelState.AddModelError("", "El monto debe ser positivo");
            }
            return View(transaccion);
        }


        // Consignar
        public IActionResult Consignar(int id)
        {
            var transaccion = new Transaccione { IdUsuario = id, Tipo = "Consignacion" };
            return View(transaccion);
        }

        [HttpPost]
        public async Task<IActionResult> Consignar(Transaccione transaccion)
        {
            var usuario = await _context.Usuarios.FindAsync(transaccion.IdUsuario);
            if (transaccion.Monto > 0)
            {
                usuario.Saldo += transaccion.Monto;
                transaccion.Fecha = DateTime.Now;
                _context.Transacciones.Add(transaccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Menu), new { id = usuario.IdUsuario });
            }
            ModelState.AddModelError("", "El monto debe ser positivo");
            return View(transaccion);
        }



        // Consultar Movimientos
        public IActionResult ConsultarMovimientos(int id)
        {
            var movimientos = _context.Transacciones.Where(t => t.IdUsuario == id).ToList();
            return View(movimientos);
        }

        [HttpPost]
        public IActionResult CerrarSesion()
        {
            // Limpiar cualquier dato de sesión o estado del usuario
            HttpContext.Session.Clear();

            // Redirigir a la página de inicio de sesión
            return RedirectToAction("Iniciar");
        }

        public IActionResult Menu(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            return View(usuario);
        }

    }
}