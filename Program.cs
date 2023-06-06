// See https://aka.ms/new-console-template for more information

using Paralelismo.Console;
while (true)
{

    var service = new Service();

    Console.WriteLine("Pararelismo");
    Console.WriteLine("1-Sem paralelismo");
    Console.WriteLine("2-Thread");
    Console.WriteLine("3-Task");
    Console.WriteLine("4-Async/Await");
    Console.WriteLine("5-CancellationToken");

    var tipo = (Tipo)Enum.Parse(typeof(Tipo), Console.ReadLine());


    await service.Executar(tipo);


}