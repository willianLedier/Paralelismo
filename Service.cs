using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paralelismo.Console
{
    public class Service
    {

        public static List<string> Tempos { get; set; } = new List<string>();

        public async Task Executar(Tipo tipo)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            switch (tipo)
            {
                case Tipo.SemParalelismo:
                    SemParalelismo();
                    break;
                case Tipo.ComThread:
                    ComThread();
                    break;
                case Tipo.ComTask:
                    ComTask();
                    break;
                case Tipo.ComAsyncAwait:
                    await ComAsyncAwait();
                    break;
                case Tipo.ComCancellationToken:
                    await ComCancellationToken();
                    break;
                default:
                    break;
            }

            stopwatch.Stop();

            var tempo = tipo.ToString() + " - Tempo decorrido: " + stopwatch.Elapsed.ToString();
            Tempos.Add(tempo);

            System.Console.WriteLine();

            foreach (var item in Tempos)
            {
                System.Console.WriteLine(item);
            }

            System.Console.WriteLine();

        }

        public void SemParalelismo()
        {
            for (int i = 1; i <= 8; i++)
            {
                Tarefa(i);
            }
        }

        public void ComThread()
        {

            var threads = new List<Thread>();

            for (int i = 1; i <= 8; i++)
            {
                int index = i;
                var thread = new Thread(() => Tarefa(index));
                thread.Start();
                threads.Add(thread);
            }

            threads.ForEach(thread => thread.Join());

        }

        public void ComTask()
        {

            var tasks = new List<Task>();
            for (int i = 1; i <= 8; i++)
            {
                int index = i;
                var task = Task.Run(() => Tarefa(index));
                tasks.Add(task);
            }
            // Aguardar a conclusão de todas as tarefas
            Task.WaitAll(tasks.ToArray());
        }

        public async Task ComAsyncAwait()
        {
            var tasks = new List<Task>();
            for (int i = 1; i <= 8; i++)
            {
                int index = i;
                var task = Task.Run(() => Tarefa(index));
                tasks.Add(task);
            }
            // Aguardar a conclusão de todas as tarefas
            await Task.WhenAll(tasks.ToArray());
        }

        public async Task ComCancellationToken()
        {

            try
            {
                // Crie uma instância de CancellationTokenSource
                CancellationTokenSource cts = new CancellationTokenSource();

                // Obtenha o token de cancelamento
                CancellationToken ct = cts.Token;

                System.Console.WriteLine("Pressione qualquer tecla para cancelar o processamento.");
                Thread.Sleep(1000);

                // Crie um array de tarefas
                var tasks = new List<Task>();

                for (int i = 1; i <= 8; i++)
                {
                    var index = i;

                    tasks.Add(Task.Run(() =>
                    {
                        
                        ct.ThrowIfCancellationRequested();

                        // Execute alguma tarefa assíncrona aqui
                        if (!ct.IsCancellationRequested)
                        {
                            Tarefa(index, ct);
                        }

                        // Verifique se o cancelamento foi solicitado
                        ct.ThrowIfCancellationRequested();

                    }, ct));

                }

                // Aguarde a entrada do usuário para cancelar as tarefas
                System.Console.ReadKey();

                // Solicite o cancelamento
                cts.Cancel();

                // Aguarde até que todas as tarefas sejam concluídas ou canceladas
                await Task.WhenAll(tasks);

            }
            catch (OperationCanceledException)
            {
                System.Console.WriteLine("------ Tarefas canceladas. -------");
            }
            
        }

        public void Tarefa(int idTarefa, CancellationToken ct = default)
        {
            System.Console.WriteLine($"Executando tarefa {idTarefa}...");

            for (int i = 1; i <= 5; i++)
            {
                ct.ThrowIfCancellationRequested();
                System.Console.WriteLine($"Tarefa {idTarefa} - Processo {i}");
                Thread.Sleep(500);
            }
            // Simulando um atraso
            Thread.Sleep(500);
            System.Console.WriteLine($"Tarefa {idTarefa} concluída.");
        }

    }

    public enum Tipo
    {
        SemParalelismo = 1,
        ComThread,
        ComTask,
        ComAsyncAwait,
        ComCancellationToken
    }

}
