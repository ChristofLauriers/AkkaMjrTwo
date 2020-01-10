CREATE DATABASE GameEngine;
GO

USE [GameEngine]
GO

IF  EXISTS (SELECT * FROM sys.schemas WHERE name = N'Event')
DROP SCHEMA [Event]
GO

CREATE SCHEMA [Event] AUTHORIZATION [dbo]
GO