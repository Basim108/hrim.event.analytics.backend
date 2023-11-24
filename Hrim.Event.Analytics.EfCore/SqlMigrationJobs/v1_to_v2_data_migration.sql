begin transaction;
alter table v2_hrim_analytics.hrim_users add column if not exists v1_id uuid;
alter table v2_hrim_analytics.event_types add column if not exists v1_id uuid;

-- migrating users and profiles
alter sequence v2_hrim_analytics.hrim_users_id_seq restart with 1;
insert into v2_hrim_analytics.hrim_users (created_at, updated_at, is_deleted, concurrent_token, v1_id)
select created_at, updated_at, is_deleted, concurrent_token, id as v1_id
from hrim_analytics.hrim_users;

alter sequence v2_hrim_analytics.external_user_profiles_id_seq restart with 1;
insert into v2_hrim_analytics.external_user_profiles
(user_id, external_user_id, idp, email, last_login, full_name, first_name, last_name,
 created_at, updated_at, is_deleted, concurrent_token)
select v2_users.id,
       v1_profile.external_user_id, v1_profile.idp, v1_profile.email, v1_profile.last_login,
       v1_profile.full_name, v1_profile.first_name, v1_profile.last_name, v1_profile.created_at, v1_profile.updated_at,
       v1_profile.is_deleted, v1_profile.concurrent_token
from hrim_analytics.external_user_profiles v1_profile
         inner join hrim_analytics.hrim_users v1_users on v1_profile.user_id = v1_users.id
         inner join v2_hrim_analytics.hrim_users as v2_users on v1_users.id = v2_users.v1_id;

-- migrating features and tags
insert into v2_hrim_analytics.hrim_features select * from v2_hrim_analytics.hrim_features;
alter sequence v2_hrim_analytics.hrim_tags_id_seq restart with 1;

-- migrating event types and analysis configurations
alter sequence v2_hrim_analytics.event_types_id_seq restart with 1;

insert into v2_hrim_analytics.event_types (tree_node_path, name, description, color, is_public, created_by,
                                           created_at, updated_at, is_deleted, concurrent_token, v1_id)
select '' as tree_node_path, v1_event_type.name, v1_event_type.description, v1_event_type.color, v1_event_type.is_public,
       v2_users.id as created_by, v1_event_type.created_at, v1_event_type.updated_at, v1_event_type.is_deleted,
       v1_event_type.concurrent_token, v1_event_type.id as v1_id
from hrim_analytics.event_types as v1_event_type
         inner join v2_hrim_analytics.hrim_users as v2_users on v1_event_type.created_by = v2_users.v1_id;

update v2_hrim_analytics.event_types set tree_node_path = id::varchar(255)::ltree;

insert into v2_analysis.analysis_config_by_event_type (event_type_id, analysis_code, is_on, settings, created_at, updated_at, concurrent_token)
select v2_event_types.id, v1.analysis_code, v1.is_on, v1.settings, v1.created_at, v1.updated_at, v1.concurrent_token
from analysis.analysis_by_event_type as v1
         inner join  v2_hrim_analytics.event_types v2_event_types on v1.event_type_id = v2_event_types.v1_id;

-- migrating events

insert into v2_hrim_analytics.duration_events (event_type_id, started_on, started_at, finished_on, finished_at, created_at, updated_at,
                                               is_deleted, concurrent_token, created_by, props)
select v2_event_types.id, v1.started_on, v1.started_at, v1.finished_on, v1.finished_at, v1.created_at, v1.updated_at,
       v1.is_deleted, v1.concurrent_token, v2_users.id as created_by, v1.props
from hrim_analytics.duration_events as v1
         inner join  v2_hrim_analytics.event_types v2_event_types on v1.event_type_id = v2_event_types.v1_id
         inner join  v2_hrim_analytics.hrim_users v2_users on v1.created_by = v2_users.v1_id;


insert into v2_hrim_analytics.occurrence_events (event_type_id, occurred_on, occurred_at,created_at, updated_at,
                                                 is_deleted, concurrent_token, created_by, props)
select v2_event_types.id, v1.occurred_on, v1.occurred_at, v1.created_at, v1.updated_at,
       v1.is_deleted, v1.concurrent_token, v2_users.id as created_by, v1.props
from hrim_analytics.occurrence_events as v1
         inner join  v2_hrim_analytics.event_types v2_event_types on v1.event_type_id = v2_event_types.v1_id
         inner join  v2_hrim_analytics.hrim_users v2_users on v1.created_by = v2_users.v1_id;

commit;