
using System;
using Mongo_Api.Data.Collections;
using Mongo_Api.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace Mongo_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfectadoController : ControllerBase
    {
        Data.MongoDB _mongoDB;
        IMongoCollection<Infectado> _infectadosCollection;

        public InfectadoController(Data.MongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _infectadosCollection = _mongoDB.DB.GetCollection<Infectado>(typeof(Infectado).Name.ToLower());
        }

        [HttpPost]
        public ActionResult SalvarInfectado([FromBody] InfectadoDTO dto)
        {
            var infectado = new Infectado(dto.DataNascimento, dto.Sexo, dto.Latitude, dto.Longitude);

            _infectadosCollection.InsertOne(infectado);
            
            return StatusCode(201, infectado);
        }

        [HttpGet]
        public ActionResult ObterInfectados()
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Empty).ToList();
            
            return Ok(infectados);
        }
        
        [HttpGet("{dataNasc}")]
        public ActionResult ObterUmInfectado(DateTime dataNasc)
        {
            var infectado = _infectadosCollection.Find(Builders<Infectado>.Filter.Where(_ => _.DataNascimento == dataNasc)).ToList();

            if (infectado.Count == 0)
            {
                return StatusCode(400, "Infectado não encontrado");
            }

            return Ok(infectado);
        }

        [HttpDelete("{dataNasc}")]
        public ActionResult DeletarInfectado(DateTime dataNasc)
        {
            var infectado = _infectadosCollection.Find(Builders<Infectado>.Filter.Where(_ => _.DataNascimento == dataNasc)).ToList();
            
            if (infectado.Count == 0)
            {
                return StatusCode(400, "Infectado não encontrado");
            }
            
            _infectadosCollection.DeleteOne(Builders<Infectado>.Filter.Where(_ => _.DataNascimento == dataNasc));            
            
            return StatusCode(204, "Deletado com sucesso");
        }
        
        [HttpPut("{dataNasc}")]
        public ActionResult AtualizarInfectado([FromBody] InfectadoDTO dto, DateTime dataNasc)
        {
            
            var infectado = _infectadosCollection.Find(Builders<Infectado>.Filter.Where(_ => _.DataNascimento == dataNasc)).ToList();
            
            if (infectado.Count == 0)
            {
                return StatusCode(400, "Infectado não encontrado");
            }
            
            _infectadosCollection.UpdateOne(
                Builders<Infectado>.Filter.Where(_ => _.DataNascimento == dataNasc),
                Builders<Infectado>.Update
                    .Set("sexo", dto.Sexo)
                    .Set("latitude", dto.Latitude)
                    .Set("longitude", dto.Longitude)
                );

            return StatusCode(202, "Atualizado com sucesso");
        }
    }
}