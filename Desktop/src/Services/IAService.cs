using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SistemaChamados.Services
{
    public class IAService
    {
        private const string WEBHOOK_URL = "https://n8n.srv993727.hstgr.cloud/webhook/ia";
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<IAResponse> SendToN8nToIa(
            string idUsuario,
            string titulo,
            string nomeUsuario,
            string email,
            string categoria,
            string descricao,
            string pessoasAfetadas,
            string bloqueiaTrabalho,
            string prioridadeParaUsuario,
            string porquePrioridadeUsuario,
            int pedaco)
        {
            try
            {
                // 1. CONVERSÃO SEGURA DE ID
                int idUsuarioNumero;
                int.TryParse(idUsuario, out idUsuarioNumero); // Se falhar, vira 0

                // 🛑 TRAVA DE SEGURANÇA (PIECE 2)
                // Se for salvar, não permite ID 0 ou inválido
                if (pedaco == 2 && idUsuarioNumero <= 0)
                {
                    Console.WriteLine("❌ Erro: Tentativa de salvar sem ID de usuário válido.");
                    return null; // Retorna erro para o Form tratar
                }

                // 2. PAYLOAD (IGUAL AO MOBILE/WEB)
                var dataPayload = new
                {
                    id_usuario = idUsuarioNumero,
                    title = titulo,
                    employeeName = nomeUsuario,
                    email = email,
                    category = categoria,
                    description = descricao,
                    affectedPeople = pessoasAfetadas,
                    blocksWork = bloqueiaTrabalho,
                    userPriority = prioridadeParaUsuario,
                    userPriorityReason = porquePrioridadeUsuario, // Nome correto para o N8N
                    piece = pedaco
                };

                string jsonPayload = JsonConvert.SerializeObject(dataPayload);
                Console.WriteLine($"📤 Enviando IA (Piece {pedaco}): {jsonPayload}");

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(WEBHOOK_URL, content);
                string rawJson = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📥 Resposta Bruta (Piece {pedaco}): {rawJson}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Erro HTTP {response.StatusCode}: {rawJson}");
                    return null;
                }

                // 3. PARSING INTELIGENTE (IGUAL AO JAVASCRIPT)
                // Resolve o problema de Array vs Objeto e Markdown
                JObject result = null;
                try
                {
                    // Limpa ```json e espaços
                    string clean = rawJson.Trim();
                    if (clean.StartsWith("```json")) clean = clean.Replace("```json", "").Replace("```", "");
                    else if (clean.StartsWith("```")) clean = clean.Replace("```", "");
                    clean = clean.Trim();

                    var token = JToken.Parse(clean);

                    if (token is JArray array)
                    {
                        result = (JObject)array.First; // Pega o primeiro item [0]
                    }
                    else if (token is JObject obj)
                    {
                        result = obj;
                    }
                }
                catch
                {
                    // Se falhar JSON, mas for salvar e tiver "registrou" no texto, aceita
                    if (pedaco == 2 && rawJson.Contains("registrou"))
                    {
                        return new IAResponse { Success = true };
                    }
                    Console.WriteLine("⚠️ Falha ao ler JSON.");
                    return null;
                }

                if (result == null) return null;

                // Checagem de erro genérico do N8N
                if (result["status"]?.ToString() == "Deu algum erro")
                {
                    Console.WriteLine("⛔ O N8N retornou erro de processamento.");
                    return null;
                }

                // ======================================================
                // PIECE 2: SALVAR
                // ======================================================
                if (pedaco == 2)
                {
                    string status = result["status"]?.ToString() ?? "";
                    // Aceita "registrou", "sucesso" ou mensagem no rawJson
                    if (status == "registrou" || status == "sucesso" || rawJson.Contains("registrou"))
                    {
                        return new IAResponse { Success = true };
                    }
                    return null;
                }

                // ======================================================
                // PIECE 3: ATRIBUIR (Mantido do seu original)
                // ======================================================
                if (pedaco == 3)
                {
                    if (result["status"]?.ToString() == "atribuido")
                    {
                        return new IAResponse
                        {
                            Success = true,
                            IdTecnico = result["id_tecnico"]?.ToString(),
                            NomeTecnico = result["nome_tecnico"]?.ToString()
                        };
                    }
                    return null;
                }

                // ======================================================
                // PIECE 1: ANÁLISE
                // ======================================================

                // Lógica "Ou um Ou outro" (Igual ao JS: prioridade || userPriority)
                string prioridade = (string)result["prioridade"]
                                 ?? (string)result["Prioridade"]
                                 ?? (string)result["userPriority"]
                                 ?? "";

                string justificativa = (string)result["Justificativa"]
                                    ?? (string)result["justificativa"]
                                    ?? (string)result["userPriorityReason"]
                                    ?? "Análise automática";

                // Normalização visual (Para garantir Maiúsculas bonitas no C#)
                prioridade = prioridade.Trim();
                string pLower = prioridade.ToLower();
                if (pLower.Contains("baixa") || pLower.Contains("low")) prioridade = "Baixa";
                else if (pLower.Contains("média") || pLower.Contains("media")) prioridade = "Média";
                else if (pLower.Contains("alta") || pLower.Contains("high")) prioridade = "Alta";
                else if (pLower.Contains("crítica") || pLower.Contains("critica")) prioridade = "Crítica";
                else if (string.IsNullOrEmpty(prioridade)) prioridade = "Média"; // Fallback

                Console.WriteLine($"✅ Dados Processados: {prioridade} | {justificativa}");

                return new IAResponse
                {
                    Success = true,
                    Prioridade = prioridade,
                    Justificativa = justificativa,
                    RawJson = rawJson
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro CRÍTICO: {ex.Message}");
                // Retorna nulo ou objeto de erro para não travar o app
                return null;
            }
        }
    }

    public class IAResponse
    {
        public bool Success { get; set; }
        public string Prioridade { get; set; }
        public string Justificativa { get; set; }
        public string RawJson { get; set; }
        public string IdTecnico { get; set; }
        public string NomeTecnico { get; set; }
    }
}