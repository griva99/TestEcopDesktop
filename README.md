# Introducción

Este proyecto ha sido creado utilizando Windows Presentation Foundation (WPF) con conexión a una base de datos SQL Server Express. Este documento proporciona instrucciones detalladas sobre cómo configurar y ejecutar el proyecto, incluyendo la creación de las tablas necesarias y la configuración de la cadena de conexión.

## Requisitos Previos

Antes de empezar, asegúrate de tener instalados los siguientes programas:

- [Visual Studio](https://visualstudio.microsoft.com/) con soporte para .NET Desktop Development.
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (versión X.X o superior).
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) o cualquier otra herramienta de administración de bases de datos compatible con SQL Server.

## Creación de la Base de Datos

### Paso 1: Crear la Base de Datos

1. Abre SQL Server Management Studio (SSMS) y conéctate a tu instancia de SQL Server.
2. Ejecuta el siguiente script para crear la base de datos:
```sql
CREATE DATABASE MiBaseDeDatos;  
```
### Paso 2: Crear las Tablas Necesarias

1. Selecciona la base de datos recién creada.
2. Ejecuta los siguientes scripts para crear las tablas necesarias:
```sql
CREATE TABLE tipodoc (
    id INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(200) NOT NULL
);
CREATE TABLE undme (
    id INT IDENTITY(1,1) PRIMARY KEY,
    descripcion VARCHAR(200) NOT NULL
);
CREATE TABLE clientes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo INT NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    id_doc INT NOT NULL,
    nrodoc VARCHAR(50) NOT NULL,
    FOREIGN KEY (id_doc) REFERENCES tipodoc(id)
);

CREATE TABLE productos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    codigo INT NOT NULL,
    descripcion VARCHAR(200) NOT NULL,
    id_und INT NOT NULL,
    precio FLOAT NOT NULL,
    FOREIGN KEY (id_und) REFERENCES undme(id)
);

CREATE TABLE pedidos (
    id INT IDENTITY(1,1) PRIMARY KEY,
    preciototal FLOAT NOT NULL,
    cantidad INT NOT NULL,
	fecha DATETIME NOT NULL
);
CREATE TABLE detallepedido (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_pedido INT NOT NULL,
    id_producto INT NOT NULL,
    id_cliente INT NOT NULL,
    precio FLOAT NOT NULL,
    preciototal FLOAT NOT NULL,
    FOREIGN KEY (id_pedido) REFERENCES pedidos(id),
    FOREIGN KEY (id_producto) REFERENCES productos(id),
    FOREIGN KEY (id_cliente) REFERENCES clientes(id)
);
--para la version web cree esta tabla temporal la cual cada que se guarda un registro esta tabla se vacia
CREATE TABLE temp_detalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_producto INT NOT NULL,
    id_cliente INT NOT NULL,
	nombre varchar (50) not null,
	descripcion varchar (200) not null,
    precio FLOAT NOT NULL,
    preciototal FLOAT NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES productos(id),
    FOREIGN KEY (id_cliente) REFERENCES clientes(id)
);
--creamos registros para pruebas
INSERT INTO productos (codigo, descripcion, id_und, precio) VALUES (1, 'coca 500ml', 1, 5000);

INSERT INTO clientes (codigo, nombre, apellido, id_doc, nrodoc) VALUES (1, 'Juan', 'Perez', 1, '12345678');
INSERT INTO tipodoc (descripcion) VALUES ('CI');
INSERT INTO tipodoc (descripcion) VALUES ('PASAPORTE');
INSERT INTO undme (descripcion) VALUES ('KILO');
INSERT INTO undme (descripcion) VALUES ('LITRO');
INSERT INTO undme (descripcion) VALUES ('UNIDAD');
```
## Configuración de la Conexión
### Paso 1: Clonar el Proyecto
Clona este repositorio en tu máquina local usando el siguiente comando:
```bash
git clone https://github.com/griva99/TestEcopDesktop.git
```
### Paso 2: Configurar la ConnectionString
Al clonar el proyecto en Visual Studio, dirígete a la clase ConexionDB. Aquí encontrarás un ejemplo de cómo configurar la cadena de conexión. Asegúrate de reemplazar los valores con los detalles de tu servidor SQL.
```csharp
// ConexionDB.cs

using System.Data.SqlClient;

namespace TuProyecto
{
    public class ConexionDB
    {
        public static string ConexionString()
        {
            // Reemplaza "your_server" por el nombre de tu servidor SQL Server
            // Reemplaza "your_database" por el nombre de tu base de datos
            public static string connectionString = "Data Source=your_server;Initial Catalog=your_database;Integrated Security=True";
        }
    }
}
```
## Ejecución del Proyecto
### Paso 1: Abrir el Proyecto en Visual Studio
1. Abre Visual Studio.
2. Selecciona "Open a project or solution".
3. Navega a la carpeta donde clonaste el repositorio y selecciona el archivo .sln.
### Paso 2: Ejecutar la Aplicación
1. Asegúrate de que tu instancia de SQL Server esté corriendo.
2. En Visual Studio, selecciona "Start" (o presiona F5) para compilar y ejecutar la aplicación.
## Funcionalidades del Proyecto
### Menú de Navegación
El proyecto incluye un menú de navegación con las siguientes secciones:

`Pedidos`: Página principal de la aplicación.

`Cliente`: Página para gestionar los clientes.

`Producto`: Página para gestionar los productos.

**Pantalla de Productos**

En la pantalla de productos, puedes realizar las siguientes acciones:

Agregar Producto: Rellena los campos y haz clic en "Guardar".
Actualizar Producto: Selecciona un producto de la grilla, carga los datos en el formulario y haz clic en "Actualizar".
Eliminar Producto: Selecciona un producto de la grilla y haz clic en "Eliminar".

**Pantalla de Clientes**

En la pantalla de clientes, puedes realizar las siguientes acciones:

Agregar cliente: Rellena los campos y haz clic en "Guardar".
Actualizar cliente: Selecciona un cliente de la grilla, carga los datos en el formulario y haz clic en "Actualizar".
Eliminar cliente: Selecciona un cliente de la grilla y haz clic en "Eliminar".

**Pantalla de Pedidos**

En la pantalla de pedidos, puedes realizar las siguientes acciones:

Agregar Producto al Pedido: Selecciona un cliente y un producto, luego haz clic en "Agregar".
Eliminar Producto del Pedido: Selecciona un producto de la grilla y haz clic en "Eliminar".
Guardar Pedido: Al final, haz clic en "Guardar" para almacenar el pedido y sus detalles en la base de datos.
La grilla muestra los productos agregados al pedido, con botones para eliminar cada producto. También muestra el precio total del pedido y un botón para guardar el pedido. Y para los pedidos es necesario agregar algun cliente.
