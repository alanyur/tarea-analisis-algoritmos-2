# Tarea 2: Análisis de Algoritmos - Distancia Mínima entre todo par de nodos

Este repositorio contiene la implementación y los experimentos comparativos entre el Algoritmo Base (múltiples ejecuciones de Bellman-Ford) y el Algoritmo de Floyd-Warshall, programados en C#. Adicionalmente, incluye un script en Python para la visualización de los resultados.

## Requisitos Previos

Para ejecutar los códigos de manera correcta, el entorno debe contar con:
* **.NET SDK 8.0** (o superior) para compilar y ejecutar el código en C#.
* **Python 3.x** para ejecutar el script de graficación.
* Librerías de Python: `pandas`, `matplotlib`, `scipy`.
    * *Comando de instalación:* `python -m pip install pandas matplotlib scipy`

## Estructura del Directorio

Antes de ejecutar el programa, asegúrese de que los siguientes archivos se encuentren en la misma carpeta principal del proyecto:
1. `Program.cs`: Contiene el código fuente principal con los algoritmos y experimentos.
2. `chebyshev2.mtx`: Dataset real extraído de NetworkRepository (necesario para la sección 2.3).
3. `graficas.py`: Script de Python para generar las curvas asintóticas.

---

## 1. Instrucciones de Ejecución (Algoritmos y Experimentos)

El código fuente está diseñado como una aplicación de consola en C#. No requiere librerías externas de grafos.

**Opción A: Ejecución mediante Terminal (Recomendado)**
1. Abra una terminal (PowerShell, CMD, o Bash) y navegue hasta el directorio del proyecto.
2. Asegúrese de que el entorno esté inicializado como proyecto de consola (`dotnet new console`).
3. Ejecute el programa con el siguiente comando:
   ```bash
   dotnet run
   
## 2. Instrucciones para la Generación de Gráficos

Una vez que el programa en C# haya finalizado exitosamente y generado el archivo `experimentos_resultados.csv`, proceda a generar los gráficos de la sección 2.2.

1. Manténgase en la misma terminal y directorio.
2. Ejecute el script de Python con el siguiente comando:
   ```bash
   python graficas.py
   ```

### Resultados Esperados de Python
El script leerá los datos del archivo CSV generado previamente y abrirá una ventana interactiva mostrando cuatro gráficos (uno por cada tipo de densidad). Además, guardará automáticamente una imagen consolidada llamada `graficas_comparativas.png` en el mismo directorio, la cual contiene los resultados empíricos contrastados con sus respectivos ajustes asintóticos teóricos.
