using System;
using System.Collections.Generic;
using Contratos;
using LogicaDeNegocios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicios;

namespace UnitTestProject1
{
    [TestClass]
    public class TestAutoMapperYCreacionTablas
    {
        //https://docs.microsoft.com/en-us/visualstudio/test/run-unit-tests-with-test-explorer?view=vs-2019
        //https://docs.telerik.com/teststudio/general-information/test-execution/vs-test-explorer
        [TestMethod]
        public void ProbarAutomoapperDeberiaDevolverObjetoLogicaDirectora()
        {
            Directora directora = new Directora()
            {
                Id = 5,
                Apellido = "Cagnino",
                Cargo = "Directora",
                Email = "directora@hotmail.com",
                FechaIngreso = DateTime.Today,
                Institucion = new Institucion()
                {
                    Direccion = "añlsdkfj",
                    Ciudad = "Rafaela",
                    Id = 6,
                    Nombre = "UCSE",
                    Provincia = "Santa Fe",
                    Telefono = "03492565738"
                },
                Nombre = "Cecilia",
            };

            LogicaDirectora DirectoraLogica = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);

            Assert.AreEqual("directora@hotmail.com", DirectoraLogica.Email);
            Assert.AreEqual(6, DirectoraLogica.Institucion.Id);
            Assert.AreEqual("Cagnino", DirectoraLogica.Apellido);
        }

        [TestMethod]
        public void ProbarAutomoapperDeberiaDevolverListaDeLogicaDirectora()
        {
            Directora directora = new Directora()
            {
                Id = 5,
                Apellido = "Cagnino",
                Cargo = "Directora",
                Email = "directora@hotmail.com",
                FechaIngreso = DateTime.Today,
                Institucion = new Institucion()
                {
                    Direccion = "añlsdkfj",
                    Ciudad = "Rafaela",
                    Id = 6,
                    Nombre = "UCSE",
                    Provincia = "Santa Fe",
                    Telefono = "03492565738"
                },
                Nombre = "Cecilia",
            };
            Directora directora2 = new Directora()
            {
                Id = 2,
                Apellido = "Sanchez",
                Cargo = "Directora",
                Email = "directoraSanchez@hotmail.com",
                FechaIngreso = DateTime.Today,
                Institucion = new Institucion()
                {
                    Direccion = "añlsasdfadkfj",
                    Ciudad = "Sunchales",
                    Id = 20,
                    Nombre = "UCSE",
                    Provincia = "Santa Fe",
                    Telefono = "03434565738"
                },
                Nombre = "Roberta",
            };
            List<Directora> lista = new List<Directora>();
            lista.Add(directora2);
            lista.Add(directora);

            var ResultadoLogica = AutoMapper.Instancia.ConvertirLista<Directora, LogicaDirectora>(lista);

            Assert.AreEqual("Roberta", ResultadoLogica[0].Nombre);
            Assert.AreEqual("Cecilia", ResultadoLogica[1].Nombre);
            Assert.AreEqual(20, ResultadoLogica[0].Institucion.Id);
            Assert.AreEqual(6, ResultadoLogica[1].Institucion.Id);
            var DirectoraEsperada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
            Assert.AreEqual(DirectoraEsperada.Id, ResultadoLogica[1].Id);
        }
        [TestMethod]
        public void ProbarCreacionDeTablas_DeberiaCrearTablasEnCarpetaPrincipal()
        {
            GeneralService servicio = new GeneralService();
            servicio.ObtenerNombreGrupo();
        }
    }
}
