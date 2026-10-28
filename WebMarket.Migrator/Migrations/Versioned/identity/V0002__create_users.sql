create table identity.users
(
    id           uuid         not null,
    email        varchar(256) not null,
    display_name varchar(128) not null,
    created_at   timestamptz  not null default now(),

    constraint pk_users primary key (id)
);

create unique index ux_users_email on identity.users (lower(email));
