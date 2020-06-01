drop table if exists logs;
drop table if exists section_content;
drop table if exists resume_section;
drop table if exists resume;
drop table if exists section_type;
drop table if exists content_type;

create table if not exists logs(
	id uuid not null primary key,
	date date,
	correlation_id text,
	username text,
	level text,
	application_name text,
	instance_id text,
	request_id text,
	thread_id text,
	source_context text,
	action_params text,
	elapsed_milliseconds int,
	message text,
	message_template text,
	audit_time timestamp,
	exception text,
	machine_name text,
	properties text,
	props_text text
);
create index ix_Logs_Date on logs(date);
create index ix_Logs_Username on logs(username);
create index ix_Logs_AuditTime on logs(audit_time);
create index ix_Logs_MessageTemplate on logs(message_template);
create index ix_Logs_CorrelationId on logs(correlation_id);

create table if not exists content_type(
	id text not null primary key,
	description text not null,
	audit_time timestamp not null,
	audit_by text not null,
	event_type text not null
);

create table if not exists section_type(
	id text not null primary key,
	description text not null,
	audit_time timestamp not null,
	audit_by text not null,
	event_type text not null
);

create table if not exists resume(
	id serial not null primary key,
	user_id uuid not null,
	name text not null,
	audit_time timestamp not null,
	audit_by text not null,
	event_type text not null
);

create table if not exists resume_section(
	id serial not null primary key,
	resume_id int not null,
	section_type_id int not null,
	sequence int not null,
	audit_time timestamp not null,
	audit_by text not null,
	event_type text not null
);

create table if not exists section_content(
	id bigserial not null primary key,
	resume_section_id int not null,
	content_type_id int not null,
	description text not null,
	sequence int not null,
	audit_time timestamp not null,
	audit_by text not null,
	event_type text not null
);
