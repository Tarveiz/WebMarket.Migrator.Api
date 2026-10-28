create table catalog.product_images
(
    id                  uuid         not null,
    product_id          uuid         not null,
    storage_key         varchar(512) not null,
    preview_storage_key varchar(512) null,
    content_type        varchar(128) not null,
    size_bytes          bigint       not null,
    sort_order          integer      not null default 0,
    status              smallint     not null default 0,
    created_at          timestamptz  not null default now(),

    constraint pk_product_images primary key (id),
    constraint ux_product_images_storage_key unique (storage_key),
    constraint ck_product_images_size_bytes check (size_bytes > 0),
    constraint ck_product_images_status check (status between 0 and 3),
    constraint fk_product_images_product_id foreign key (product_id) references catalog.products (id) on delete cascade
);

create index ix_product_images_product_id on catalog.product_images (product_id, sort_order);
