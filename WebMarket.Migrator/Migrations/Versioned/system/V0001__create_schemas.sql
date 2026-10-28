create schema if not exists system;
create schema if not exists identity;
create schema if not exists catalog;

-- Триггерная функция заполнения updated_at
create function system.set_updated_at() returns trigger
language plpgsql
as $$
begin
    new.updated_at := now();
    return new;
end;
$$;
