create table catalog.categories
(
    id         uuid         not null,
    parent_id  uuid         null,
    name       varchar(128) not null,
    slug       varchar(128) not null,
    sort_order integer      not null default 0,

    constraint pk_categories primary key (id),
    constraint ux_categories_slug unique (slug),
    constraint ck_categories_parent_id check (parent_id <> id),
    constraint fk_categories_parent_id foreign key (parent_id) references catalog.categories (id)
);

create index ix_categories_parent_id on catalog.categories (parent_id, sort_order);
