using System;
using System.Collections.Generic;
using Contratos;
using Lógica_de_Negocios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Servicios;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //https://docs.microsoft.com/en-us/visualstudio/test/run-unit-tests-with-test-explorer?view=vs-2019
        //https://docs.telerik.com/teststudio/general-information/test-execution/vs-test-explorer
        [TestMethod]
        public void TestMethod1()
        {
            LogicaDirectora x = new LogicaDirectora()
            {
                Id = 5,
                Apellido = "Cagnino",
                Cargo = "Directora",
                Email = "directora@hotmail.com",
                FechaIngreso = DateTime.Today,
                Institucion = new LogicaInstitucion()
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

            //Directora res = AutoMapper.Instancia.Mapear(x, new Directora());

            //Assert.AreEqual("directora@hotmail.com", res.Email);
            //Assert.AreEqual(6, res.Institucion.Id);
            //Assert.AreEqual("Cagnino", res.Apellido);



            LogicaDirectora x2 = new LogicaDirectora()
            {
                Id = 2,
                Apellido = "Sanchez",
                Cargo = "Directora",
                Email = "directoraSanchez@hotmail.com",
                FechaIngreso = DateTime.Today,
                Institucion = new LogicaInstitucion()
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
            List<LogicaDirectora> lista = new List<LogicaDirectora>();
            lista.Add(x2);
            lista.Add(x);

            List<Directora> resultado = new List<Directora>();
            foreach (var elem in lista)
            {
                resultado.Add(AutoMapper.Instancia.Mapear(elem, new Directora()));
            }

            Assert.AreEqual("Roberta", resultado[0].Nombre);
            Assert.AreEqual("Cecilia", resultado[1].Nombre);
            Assert.AreEqual( 20, resultado[0].Institucion.Id);
            Assert.AreEqual( 6, resultado[1].Institucion.Id);
        }
    }
}
