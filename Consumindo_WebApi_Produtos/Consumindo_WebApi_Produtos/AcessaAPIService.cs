﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Consumindo_WebApi_Produtos
{
    public class AcessaAPIService 
    {
        public async Task<List<Produto>> GetAllProdutos(string URI, AccessToken accessToken)
        {
            using (var client = new HttpClient())
            {
                GetHeaderTokenAuthorization(client, accessToken);

                using (var response = await client.GetAsync((URI)))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                        List<Produto> produtos = JsonConvert.DeserializeObject<Produto[]>(ProdutoJsonString).ToList();
                        return produtos;
                    }
                    else
                    {
                        throw new Exception("Não foi possível obter o produto: " + response.StatusCode);
                    }
                }
            }
        }

        public async Task<Produto> GetProdutoById(string URI, AccessToken accessToken)
        {
            using (var client = new HttpClient())
            {
                GetHeaderTokenAuthorization(client, accessToken);
                HttpResponseMessage response = await client.GetAsync(URI);
                if (response.IsSuccessStatusCode)
                {
                    var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                    Produto produto = JsonConvert.DeserializeObject<Produto>(ProdutoJsonString);
                    return produto;
                }
                else
                {
                    throw new Exception("Falha ao obter o produto : " + response.StatusCode);
                }
            }
        }

        public async Task<string> AddProduto(string URI, AccessToken accessToken,  Produto produto)
        {
            using (var client = new HttpClient())
            {
                GetHeaderTokenAuthorization(client, accessToken);

                var serializedProduto = JsonConvert.SerializeObject(produto);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(URI, content);

                if (result.IsSuccessStatusCode)
                {
                    return "Produto incluido com sucesso";
                }
                else
                {
                    return "Falha ao incluir produto "  + result.StatusCode;
                }
            }
        }

        public async Task<string> UpdateProduto(string URI, AccessToken accessToken, Produto produto)
        {
            using (var client = new HttpClient())
            {
                GetHeaderTokenAuthorization(client, accessToken);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(URI, produto);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return "Produto atualizado";
                }
                else
                {
                    return "Falha ao atualizar o produto : " + responseMessage.StatusCode;
                }
            }
        }

        public async Task<string> DeleteProduto(string URI,AccessToken accessToken,int codProduto)
        {
            using (var client = new HttpClient())
            {
                GetHeaderTokenAuthorization(client,accessToken);
                client.BaseAddress = new Uri(URI);

                HttpResponseMessage responseMessage = await client.DeleteAsync($"{URI}/{codProduto}");
                if (responseMessage.IsSuccessStatusCode)
                {
                    return "Produto excluído com sucesso";
                }
                else
                {
                    return "Falha ao excluir o produto  : " + responseMessage.StatusCode;
                }
            }
        }

        public static void GetHeaderTokenAuthorization(HttpClient client, AccessToken accessToken)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken.Token);
        }
    }
}
