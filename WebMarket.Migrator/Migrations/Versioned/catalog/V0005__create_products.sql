create table catalog.products
(
    id             uuid          not null,
    owner_id       uuid          not null,
    category_id    uuid          not null,
    sku            varchar(64)   not null,
    name           varchar(256)  not null,
    description    text          null,
    price          numeric(18,2) not null,
    currency       char(3)       not null default 'RUB',
    stock          integer       not null default 0,
    reserved_stock integer       not null default 0,
    status         smallint      not null default 0,
    attributes     jsonb         not null default '{}',
    is_deleted     boolean       not null default false,
    created_by     uuid          not null,
    created_at     timestamptz   not null default now(),
    updated_at     timestamptz   not null default now(),

    constraint pk_products primary key (id),
    constraint ck_products_price check (price >= 0),
    constraint ck_products_currency check (currency ~ '^[A-Z]{3}$'),
    constraint ck_products_stock check (reserved_stock >= 0 and stock >= reserved_stock),
    constraint ck_products_status check (status between 0 and 3),
    constraint ck_products_attributes check (jsonb_typeof(attributes) = 'object'),
    constraint fk_products_owner_id foreign key (owner_id) references catalog.owners (id),
    constraint fk_products_category_id foreign key (category_id) references catalog.categories (id),
    constraint fk_products_created_by foreign key (created_by) references identity.users (id)
);

-- Артикул уникален среди неудалённых товаров владельца
create unique index ux_products_owner_id_sku on catalog.products (owner_id, sku) where not is_deleted;

-- Каталог: keyset-пагинация по (created_at, id)
create index ix_products_catalog on catalog.products (category_id, status, created_at desc, id desc) where not is_deleted;

-- Товары владельца
create index ix_products_owner_id on catalog.products (owner_id, created_at desc, id desc) where not is_deleted;

create trigger tr_products_set_updated_at before update on catalog.products for each row execute function system.set_updated_at();
