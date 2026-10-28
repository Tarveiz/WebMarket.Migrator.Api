create table catalog.owners
(
    id         uuid         not null,
    name       varchar(256) not null,
    slug       varchar(128) not null,
    status     smallint     not null default 0,
    created_by uuid         not null,
    created_at timestamptz  not null default now(),
    updated_at timestamptz  not null default now(),

    constraint pk_owners primary key (id),
    constraint ux_owners_slug unique (slug),
    constraint ck_owners_status check (status between 0 and 1),
    constraint fk_owners_created_by foreign key (created_by) references identity.users (id)
);

create index ix_owners_created_by on catalog.owners (created_by);

create trigger tr_owners_set_updated_at before update on catalog.owners for each row execute function system.set_updated_at();

create table catalog.owner_members
(
    owner_id   uuid        not null,
    user_id    uuid        not null,
    role       smallint    not null,
    created_at timestamptz not null default now(),

    constraint pk_owner_members primary key (owner_id, user_id),
    constraint ck_owner_members_role check (role between 0 and 1),
    constraint fk_owner_members_owner_id foreign key (owner_id) references catalog.owners (id) on delete cascade,
    constraint fk_owner_members_user_id foreign key (user_id) references identity.users (id) on delete cascade
);

create index ix_owner_members_user_id on catalog.owner_members (user_id);
