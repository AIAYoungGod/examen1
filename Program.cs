using System;
using System.Text.RegularExpressions;
using System.Globalization;

class Program
{
    static void Main()
    {
        string nombre = "";
        string profesion = "";
        int entrada = 0, salida = 0;
        double tarifaHora = 0, tarifaExtra = 0;
        double salarioBruto = 0, salarioNeto = 0, bonificacion = 0, deducciones = 0;
        int totalHoras = 0, horasExtras = 0;
        bool sinFaltas = true;
        int diasTrabajados = 0, diasAusentes = 0;
        int llegadasTarde = 0, salidasTemprano = 0, horasNoTrabajadas = 0;
        const int minimoHorasDiarias = 6;
        const int limiteHorasSemanalesRecargo = 40;
        const double recargoPorHorasExtras = 0.25;
        const int horaInicioLaboral = 7;
        const int horaFinLaboral = 16;
        const double bonoSemanal = 60.0;

        while (true)
        {
            Console.WriteLine("\nMenú:");
            Console.WriteLine("1. Ingresar datos de empleado");
            Console.WriteLine("2. Mostrar reporte de pago semanal");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");
            string opcionTexto = Console.ReadLine() ?? "0";
            int opcion;
            if (!int.TryParse(opcionTexto, out opcion)) continue;

            if (opcion == 3) break;

            switch (opcion)
            {
                case 1:
                    do
                    {
                        Console.Write("Ingrese el nombre del empleado (sin números): ");
                        nombre = Console.ReadLine() ?? "Empleado";
                    } while (!Regex.IsMatch(nombre, "^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$"));

                    Console.Write("Ingrese la profesión del empleado: ");
                    profesion = Console.ReadLine() ?? "General";

                    string tarifaTexto;
                    do
                    {
                        Console.Write("Ingrese la tarifa por hora ordinaria (solo números positivos): ");
                        tarifaTexto = Console.ReadLine() ?? "0";
                    } while (!double.TryParse(tarifaTexto, out tarifaHora) || tarifaHora <= 0);

                    tarifaExtra = tarifaHora * 1.5;

                    totalHoras = 0;
                    horasExtras = 0;
                    diasTrabajados = 0;
                    diasAusentes = 0;
                    llegadasTarde = 0;
                    salidasTemprano = 0;
                    horasNoTrabajadas = 0;
                    sinFaltas = true;

                    for (int i = 1; i <= 5; i++)
                    {
                        Console.WriteLine($"\nDía {i} de la semana laboral");
                        string entradaTexto, salidaTexto;
                        do
                        {
                            Console.Write("Ingrese la hora de entrada (Formato 12h con AM/PM, ej. 8AM o 2PM o 'AUSENTE' para indicar inasistencia): ");
                            entradaTexto = Console.ReadLine()?.Trim().ToUpper() ?? "";
                        } while (!EsHoraValida(entradaTexto));

                        if (entradaTexto == "AUSENTE")
                        {
                            Console.WriteLine("Día marcado como ausente. Se pierde el bono.");
                            sinFaltas = false;
                            diasAusentes++;
                            continue;
                        }

                        entrada = ConvertirHora12a24(entradaTexto);

                        do
                        {
                            Console.Write("Ingrese la hora de salida (Formato 12h con AM/PM, ej. 5PM): ");
                            salidaTexto = Console.ReadLine()?.Trim().ToUpper() ?? "";
                        } while (!EsHoraValida(salidaTexto));

                        salida = ConvertirHora12a24(salidaTexto);
                        if (salida <= entrada)
                        {
                            Console.WriteLine("Error: La hora de salida debe ser mayor a la de entrada. Intente nuevamente.");
                            i--; // Volver a pedir el mismo día
                            continue;
                        }

                        int horasDia = salida - entrada;
                        if (horasDia < 8)
                        {
                            int horasPerdidas = 8 - horasDia;
                            horasNoTrabajadas += horasPerdidas;
                        }
                        totalHoras += horasDia;
                        if (horasDia > 8)
                        {
                            horasExtras += horasDia - 8;
                        }
                    }
                    salarioBruto = (totalHoras - horasExtras) * tarifaHora + horasExtras * tarifaExtra;
                    bonificacion = (sinFaltas && diasAusentes == 0) ? bonoSemanal : 0;
                    deducciones = (llegadasTarde * tarifaHora) + (salidasTemprano * tarifaHora) + (horasNoTrabajadas * tarifaHora);
                    salarioNeto = salarioBruto + bonificacion - deducciones;
                    break;

                case 2:
                    Console.WriteLine("\n--- Reporte de Pago Semanal ---");
                    Console.WriteLine($"Empleado: {nombre}");
                    Console.WriteLine($"Profesión: {profesion}");
                    Console.WriteLine($"Tarifa por hora: {tarifaHora:C}");
                    Console.WriteLine($"Horas trabajadas: {totalHoras}");
                    Console.WriteLine($"Horas extra: {horasExtras}");
                    Console.WriteLine($"Horas no trabajadas: {horasNoTrabajadas}");
                    Console.WriteLine($"Salario Bruto: {salarioBruto:C}");
                    Console.WriteLine($"Bonificación: {bonificacion:C}");
                    Console.WriteLine($"Deducciones: {deducciones:C}");
                    Console.WriteLine($"Salario Neto: {salarioNeto:C}");
                    break;
            }
        }
    }

    static bool EsHoraValida(string hora)
    {
        if (hora == "AUSENTE") return true;
        return Regex.IsMatch(hora, "^(0?[1-9]|1[0-2])(AM|PM)$");
    }

    static int ConvertirHora12a24(string hora12)
    {
        if (!EsHoraValida(hora12)) return -1;
        bool esPM = hora12.Contains("PM");
        int hora = int.Parse(Regex.Match(hora12, "\\d+").Value);
        if (esPM && hora != 12) hora += 12;
        if (!esPM && hora == 12) hora = 0;
        return hora;
    }
}
