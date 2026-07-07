import pandas as pd
import matplotlib.pyplot as plt
import numpy as np
from scipy.optimize import curve_fit

# Funciones teóricas para ajuste
def theory_floyd_warshall(v, c):
    return c * (v**3)

def theory_base(v, c, density_factor):
    # E = V^2 * density_factor. Complejidad = V^2 * E = V^4 * density
    # Para el grafo de cadena, E = V. Complejidad = V^3.
    # Ajustamos simplificadamente dependiendo de la densidad
    return c * (v**3) if density_factor == 0 else c * (v**4) * density_factor

# Cargar datos
df = pd.read_csv("experimentos_resultados.csv")
experimentos = df['ExperimentType'].unique()

plt.figure(figsize=(14, 10))

for i, exp in enumerate(experimentos, 1):
    plt.subplot(2, 2, i)
    data_exp = df[df['ExperimentType'] == exp]
    
    # Separar algoritmos
    data_base = data_exp[data_exp['Algorithm'] == 'Base']
    data_fw = data_exp[data_exp['Algorithm'] == 'FloydWarshall']
    
    V_vals = data_base['N'].values
    
    # Plot empírico con barras de error
    plt.errorbar(V_vals, data_base['MeanTimeMs'], yerr=data_base['StdDevMs'], 
                 label='Base (Bellman-Ford)', marker='o', capsize=5)
    plt.errorbar(V_vals, data_fw['MeanTimeMs'], yerr=data_fw['StdDevMs'], 
                 label='Floyd-Warshall', marker='s', capsize=5)

    # Ajuste Teórico Floyd-Warshall (O(V^3))
    if len(V_vals) > 1:
        popt_fw, _ = curve_fit(theory_floyd_warshall, V_vals, data_fw['MeanTimeMs'])
        plt.plot(V_vals, theory_floyd_warshall(V_vals, *popt_fw), '--', color='orange', 
                 label=r'Teórico FW $O(V^3)$')

    # Ajuste Teórico Base
    density = 0.1 if exp == "Densidad_10" else (0.9 if exp == "Densidad_90" else (1.0 if exp == "Completo" else 0))
    if len(V_vals) > 1:
        popt_base, _ = curve_fit(lambda v, c: theory_base(v, c, density), V_vals, data_base['MeanTimeMs'])
        plt.plot(V_vals, [theory_base(v, *popt_base, density) for v in V_vals], '--', color='blue', 
                 label='Teórico Base')

    plt.title(f'Experimento: {exp}')
    plt.xlabel('Cantidad de Nodos (N)')
    plt.ylabel('Tiempo Promedio (ms)')
    plt.legend()
    plt.grid(True)

plt.tight_layout()
plt.savefig("graficas_comparativas.png")
plt.show()
print("Gráficas guardadas como 'graficas_comparativas.png'")