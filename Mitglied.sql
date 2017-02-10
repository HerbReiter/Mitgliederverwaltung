USE [Mitgliederverwaltung]
GO

/****** Object:  Table [dbo].[Mitglied]    Script Date: 10.02.2017 07:22:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Mitglied](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Ort] [nvarchar](50) NOT NULL,
	[Strasse] [nvarchar](50) NOT NULL,
	[Geburtsdatum] [date] NOT NULL,
	[Geschlecht] [nvarchar](1) NOT NULL,
	[Notiz] [nvarchar](4000) NULL,
	[Telefon] [nvarchar](max) NULL,
 CONSTRAINT [PK_Mitglied] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Mitglied]  WITH CHECK ADD  CONSTRAINT [CK_Mitglied_Geschlecht] CHECK  (([Geschlecht]='M' OR [Geschlecht]='W'))
GO

ALTER TABLE [dbo].[Mitglied] CHECK CONSTRAINT [CK_Mitglied_Geschlecht]
GO


