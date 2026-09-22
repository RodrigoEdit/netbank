# Core Banking API

RESTful API transaccional para la gestión de cuentas bancarias y procesamiento de transferencias interbancarias atómicas, desarrollada con **ASP.NET Core** y persistencia relacional mediante **Entity Framework Core**.

---

## 🛠️ Tecnologías Utilizadas

- **Lenguaje & Framework:** C# (.NET Core / ASP.NET Core Web API)
- **ORM:** Entity Framework Core (Code-First)
- **Base de Datos:** SQLite
- **Documentación & Pruebas:** Swagger / OpenAPI

---

## 📌 Características Principales

- **Arquitectura RESTful:** Controladores estructurados con inyección de dependencias y códigos de estado HTTP estandarizados (`200 OK`, `400 Bad Request`, `404 Not Found`).
- **Operaciones Atómicas:** Procesamiento seguro de transferencias con actualización simultánea de cuentas y validación de saldos suficientes.
- **Auditoría e Historial:** Registro persistente de cada movimiento financiero con trazabilidad en formato UTC (`DateTime.UtcNow`).
- **Control de Reglas de Negocio:**
  - Prevención de transferencias con montos menores o iguales a cero.
  - Validación de existencia de cuentas de origen y destino.
  - Verificación de balance previo al débito.

---

## 🚀 Endpoints de la API

| Método | Ruta | Descripción |
| :--- | :--- | :--- |
| `POST` | `/api/Transferencias/transaccion` | Procesa una transferencia entre dos cuentas bancarias. |
| `GET` | `/api/Transferencias/historial/{cuenta}` | Retorna el historial de movimientos de una cuenta ordenado cronológicamente. |

### Ejemplo de Payload (Transferencia)
```json
POST /api/Transferencias/transaccion
{
  "cuentaOrigen": "PE-001-987654",
  "cuentaDestino": "PE-002-123456",
  "monto": 150.00
}
```

---

## ⚙️ Instalación y Ejecución Local

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/RodrigoEdit/netbank.git](https://github.com/RodrigoEdit/netbank.git)
   cd banco-api
   ```

2. **Restaurar dependencias y ejecutar migraciones:**
   ```bash
   dotnet restore
   dotnet ef database update
   ```

3. **Iniciar la aplicación:**
   ```bash
   dotnet run
   ```

4. **Acceder a la documentación Swagger:**
   Abrir en el navegador: `https://localhost:{puerto}/swagger`