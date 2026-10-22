-- 1. Crear la base de datos si no existe
CREATE DATABASE IF NOT EXISTS EduStreamDB;

-- 2. Seleccionar la base de datos
USE EduStreamDB;

DROP TABLE IF EXISTS ProgresoLecciones;
DROP TABLE IF EXISTS Pagos;
DROP TABLE IF EXISTS MaterialesEstudio;
DROP TABLE IF EXISTS Lecciones;
DROP TABLE IF EXISTS Inscripciones;
DROP TABLE IF EXISTS Cursos;
DROP TABLE IF EXISTS Categorias;
DROP TABLE IF EXISTS Usuarios;

-- 4. Crear las tablas
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Rol VARCHAR(50) NOT NULL, 
    AvatarUrl VARCHAR(255) NULL,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Categorias (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255) NULL,
    Estado BOOLEAN DEFAULT 1 
);

CREATE TABLE Cursos (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Titulo VARCHAR(200) NOT NULL,
    Descripcion TEXT NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    CupoMaximo INT NULL, 
    PortadaUrl VARCHAR(255) NULL,
    Estado VARCHAR(50) DEFAULT 'Activo', 
    CategoriaId INT NOT NULL,
    ProfesorId INT NOT NULL,
    FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    FOREIGN KEY (ProfesorId) REFERENCES Usuarios(Id)
);

CREATE TABLE Inscripciones (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    EstudianteId INT NOT NULL,
    CursoId INT NOT NULL,
    FechaInscripcion DATETIME DEFAULT CURRENT_TIMESTAMP,
    Estado VARCHAR(50) DEFAULT 'Activa',
    MontoPago DECIMAL(10, 2) NOT NULL,
    EstadoPago VARCHAR(50) DEFAULT 'Pendiente',
    FOREIGN KEY (EstudianteId) REFERENCES Usuarios(Id),
    FOREIGN KEY (CursoId) REFERENCES Cursos(Id)
);

CREATE TABLE Lecciones (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Titulo VARCHAR(200) NOT NULL,
    Contenido TEXT NULL,
    Orden INT NOT NULL,
    CursoId INT NOT NULL,
    FOREIGN KEY (CursoId) REFERENCES Cursos(Id)
);

CREATE TABLE MaterialesEstudio (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    NombreArchivo VARCHAR(200) NOT NULL,
    Extension VARCHAR(10) NOT NULL,
    RutaArchivo VARCHAR(500) NOT NULL,
    FechaSubida DATETIME DEFAULT CURRENT_TIMESTAMP,
    LeccionId INT NOT NULL,
    FOREIGN KEY (LeccionId) REFERENCES Lecciones(Id)
);

CREATE TABLE ProgresoLecciones (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    InscripcionId INT NOT NULL,
    LeccionId INT NOT NULL,
    Completada BOOLEAN DEFAULT 0, -- 0: Empezada pero no terminada, 1: Completada
    FechaUltimoAcceso DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (InscripcionId) REFERENCES Inscripciones(Id),
    FOREIGN KEY (LeccionId) REFERENCES Lecciones(Id)
);