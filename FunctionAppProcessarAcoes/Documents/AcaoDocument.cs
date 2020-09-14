using MongoDB.Bson;

namespace FunctionAppProcessarAcoes.Documents
{
    public class AcaoDocument
    {
        public ObjectId _id { get; set; }
        public string HistLancamento { get; set; }
        public string Sigla { get; set; }
        public string Data { get; set; }
        public double Valor { get; set; }        
        public DadosCorretora Corretora { get; set; }
    }

    public class DadosCorretora
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
    }
}