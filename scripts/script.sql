CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Active" boolean NOT NULL,
    "ImagePath" character varying(500),
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260211001909_InitialCreate', '10.0.3');

COMMIT;

START TRANSACTION;
ALTER TABLE "Products" ADD "Category" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260211002729_AddProductCategory', '10.0.3');

COMMIT;

START TRANSACTION;
CREATE INDEX idx_products_active_category_price ON "Products" ("Category", "Price") WHERE "Active" = true;

CREATE INDEX idx_products_active_price ON "Products" ("Price") WHERE "Active" = true;

CREATE INDEX idx_products_category ON "Products" ("Category");

CREATE INDEX idx_products_status ON "Products" ("Active");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260211014854_AddProductIndex', '10.0.3');

COMMIT;

