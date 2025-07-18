CREATE DATABASE FERRETERIA_JE;
USE FERRETERIA_JE;

CREATE TABLE proveedor (
    id_proveedor INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(100),
    telefono VARCHAR(15),
    email VARCHAR(100),
    PRIMARY KEY (id_proveedor)
);

CREATE TABLE producto (
    id_producto INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(255),
    precio DECIMAL(10,2) NOT NULL,
    stock INT NOT NULL DEFAULT 0,
    PRIMARY KEY (id_producto)
);

CREATE TABLE usuarios (
    id_usuario INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    PRIMARY KEY (id_usuario)
);

CREATE TABLE compra (
    id_compra INT NOT NULL AUTO_INCREMENT,
    fecha DATE NOT NULL,
    total DECIMAL(10,2) NOT NULL,
    id_proveedor INT,
    id_usuario INT,
    PRIMARY KEY (id_compra),
    FOREIGN KEY (id_proveedor) REFERENCES proveedor(id_proveedor),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario)
);

CREATE TABLE cliente (
    id_cliente INT NOT NULL AUTO_INCREMENT,
    cedula VARCHAR(20),
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(100),
    email VARCHAR(100),
    telefono VARCHAR(15),
    PRIMARY KEY (id_cliente)
);

CREATE TABLE venta (
    id_venta INT NOT NULL AUTO_INCREMENT,
    fecha DATE NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    cantidad DECIMAL(10,2) NOT NULL,
    id_cliente INT,
    id_usuario INT,
    id_producto INT,
    total DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id_venta),
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_producto) REFERENCES producto(id_producto)
);

CREATE TABLE factura (
    id_factura INT NOT NULL AUTO_INCREMENT,
    fecha DATE NOT NULL,
    id_cliente INT,
    total DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id_factura),
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

CREATE TABLE detalle_venta (
    id_detalle INT NOT NULL AUTO_INCREMENT,
    id_venta INT,
    id_producto INT,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2),
    PRIMARY KEY (id_detalle),
    FOREIGN KEY (id_venta) REFERENCES venta(id_venta),
    FOREIGN KEY (id_producto) REFERENCES producto(id_producto)
);

CREATE TABLE detalle_compra (
    id_detalle INT NOT NULL AUTO_INCREMENT,
    id_compra INT,
    id_producto INT,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2),
    PRIMARY KEY (id_detalle),
    FOREIGN KEY (id_compra) REFERENCES compra(id_compra),
    FOREIGN KEY (id_producto) REFERENCES producto(id_producto)
);

INSERT INTO usuarios (nombre, telefono, email, password, rol)
VALUES ('Administrador', '3158153052', 'admin@empresa.com', 'Admin123*', 'admin'),
 ('Auxiliar', '3153546287', 'auxiliar@empresa.com', 'Auxiliar123*', 'auxiliar'),
 ('Cajero', '3364759287', 'cajero@empresa.com', 'Cajero123*', 'cajero');

INSERT INTO proveedor (nombre, direccion, telefono, email) VALUES
('Aceros y Perfiles S.A.', 'Calle 45 # 10-20', '3101112233', 'ventas@acerosyperfiles.com'),
('Herramientas de Precisión Ltda.', 'Carrera 20 # 5-15', '3002223344', 'contacto@herramientasprecision.com'),
('Pinturas y Acabados Color', 'Avenida 30 # 12-05', '3203334455', 'pedidos@pinturascolor.com'),
('Materiales de Construcción El Roble', 'Calle 82', '3154445566', 'info@materialeselroble.com'),
('Ferretería Central Mayorista', 'Calle 70 # 8-30', '3015556677', 'comercial@ferreteriacentral.com'),
('Tuberías y Conexiones Andina', 'Zona Industrial # 1-80', '3186667788', 'ventas@tuberiassandina.com'),
('Electricidad Segura S.A.S.', 'Diagonal 50 # 15-02', '3057778899', 'clientes@electricidadsegura.com'),
('Fijaciones y Tornillos Unidos', 'Calle 85 # 2-10', '3128889900', 'soporte@fijacionesunidos.com'),
('Equipos de Seguridad Industrial', 'Transversal 100 # 4-07', '3049990011', 'info@equiposseguridad.com'),
('Plomería y Grifería Moderna', 'Carrera 6 # 18-25', '3170001122', 'ventas@plomeriamoderna.com');

INSERT INTO producto (nombre, descripcion, precio, stock) VALUES
('martillo', 'herramienta', 5000.00, 15),
('cemento', 'material', 25000.00, 20),
('arena kilo', 'material', 3000.00, 10),
('destornillador de pala', 'herramienta', 3500.00, 7),
('destornillador de estrella', 'herramienta', 3500.00, 10),
('ducha electrica', 'herramienta', 45000.00, 5),
('alicate', 'herramienta', 7500.00, 12),
('yeso', 'material', 3500.00, 20),
('cinta electrica', 'material', 3000.00, 12),
('tubo cortina por metro', 'material', 4500.00, 12),
('metro', 'herramienta', 20000.00, 6),
('lija', 'material', 1000.00, 50),
('puntilla pared', 'material', 100.00, 500);

INSERT INTO cliente (cedula, nombre, direccion, email, telefono) VALUES
('1010123456', 'Ana María Gómez Pérez', 'Carrera 15 # 8-45', 'anamaria.gp@gmail.com', '3001234567'),
('79000123', 'Juan Carlos Ramírez Soto', 'Calle 50 # 25-10', 'juancarlos.rs@hotmail.com', '3102345678'),
('1098765432', 'María Fernanda López Díaz', 'Avenida 1 de Mayo', 'mariafernanda.ld@gmail.com', '3203456789'),
('80123456', 'Pedro Antonio Martínez Ruiz', 'Diagonal 70 # 30-05', 'pedro.amr@hotmail.com', '3014567890'),
('1020304050', 'Laura Sofía Vargas Castro', 'Calle 34 # 18-22', 'laura.svc@gmail.com', '3155678901'),
('88990011', 'Carlos Andrés Suárez Rojas', 'Carrera 10 # 6-15', 'carlos.asr@hotmail.com', '3186789012'),
('1045678901', 'Sofía Alejandra Torres Vega', 'Transversal 3 # 20-40', 'sofia.atv@gmail.com', '3057890123'),
('90123456', 'Andrés Felipe Castro Morales', 'Calle 10 # 5-08', 'andres.fcm@hotmail.com', '3128901234'),
('1076543210', 'Valentina Ruiz Herrera', 'Avenida 0 # 13-03', 'valentina.rh@gmail.com', '3049012345'),
('77889900', 'Diego Alejandro Pardo Gil', 'Carrera 7 # 100-50', 'diego.apg@hotmail.com', '3170123456');


INSERT INTO venta (fecha, precio_unitario, cantidad, id_cliente, id_usuario, id_producto, total) VALUES
('2025-07-01', 5000.00, 1, 1, 3, 1, 5000.00),
('2025-07-01', 25000.00, 2, 2, 3, 2, 50000.00), 
('2025-07-02', 3500.00, 1, 3, 3, 4, 3500.00), 
('2025-07-02', 45000.00, 1, 4, 3, 6, 45000.00),
('2025-07-03', 3500.00, 5, 5, 3, 8, 17500.00), 
('2025-07-03', 4500.00, 3, 6, 3, 10, 13500.00), 
('2025-07-04', 20000.00, 1, 7, 3, 11, 20000.00), 
('2025-07-04', 1000.00, 10, 8, 3, 12, 10000.00), 
('2025-07-05', 100.00, 100, 9, 3, 13, 10000.00), 
('2025-07-05', 3000.00, 2, 10, 3, 3, 6000.00); 

INSERT INTO compra (fecha, total, id_proveedor, id_usuario) VALUES
('2025-06-25', 500000.00, 2, 1),
('2025-06-26', 750000.00, 4, 2),
('2025-06-27', 120000.00, 3, 1),
('2025-06-28', 300000.00, 1, 2),
('2025-06-29', 80000.00, 8, 1),
('2025-07-01', 250000.00, 6, 2),
('2025-07-02', 180000.00, 7, 1),
('2025-07-03', 400000.00, 5, 2),
('2025-07-04', 95000.00, 9, 1),
('2025-07-05', 150000.00, 10, 2);

INSERT INTO factura (fecha, id_cliente, total) VALUES
('2025-07-01', 1, 5000.00),
('2025-07-01', 2, 50000.00),
('2025-07-02', 3, 3500.00),
('2025-07-02', 4, 45000.00),
('2025-07-03', 5, 17500.00),
('2025-07-03', 6, 13500.00),
('2025-07-04', 7, 20000.00),
('2025-07-04', 8, 10000.00);

select * from factura
