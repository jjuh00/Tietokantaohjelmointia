CREATE TABLE flights (
    id integer NOT NULL,
    flight_number character varying(20) NOT NULL,
    departure_airport character varying(100) NOT NULL,
    destination_airport character varying(100) NOT NULL,
    departure_time timestamp without time zone NOT NULL,
    duration numeric(5,2) NOT NULL,
    price numeric(10,2) NOT NULL,
    capacity integer NOT NULL,
    seats_available integer NOT NULL,
    CONSTRAINT flights_check CHECK (((seats_available >= 0) AND (seats_available <= capacity)))
);

CREATE SEQUENCE flights_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE flights_id_seq OWNED BY flights.id;

CREATE TABLE reservations (
    id integer NOT NULL,
    reservation_number character varying(50) NOT NULL,
    user_id integer NOT NULL,
    flight_id integer NOT NULL,
    booking_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


CREATE SEQUENCE reservations_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE reservations_id_seq OWNED BY reservations.id;

CREATE TABLE users (
    id integer NOT NULL,
    name character varying(100) NOT NULL,
    email character varying(150) NOT NULL,
    password character varying(255) NOT NULL,
    role smallint DEFAULT 0 NOT NULL
);

CREATE SEQUENCE users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

ALTER SEQUENCE users_id_seq OWNED BY users.id;

SELECT pg_catalog.setval('flights_id_seq', 8, true);
SELECT pg_catalog.setval('reservations_id_seq', 3, true);
SELECT pg_catalog.setval('users_id_seq', 4, true);

ALTER TABLE ONLY flights
    ADD CONSTRAINT flights_flight_number_key UNIQUE (flight_number);
ALTER TABLE ONLY flights
    ADD CONSTRAINT flights_pkey PRIMARY KEY (id);
ALTER TABLE ONLY reservations
    ADD CONSTRAINT reservations_pkey PRIMARY KEY (id);
ALTER TABLE ONLY reservations
    ADD CONSTRAINT reservations_reservation_number_key UNIQUE (reservation_number);
ALTER TABLE ONLY users
    ADD CONSTRAINT users_email_key UNIQUE (email);
ALTER TABLE ONLY users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
ALTER TABLE ONLY reservations
    ADD CONSTRAINT fk_flight FOREIGN KEY (flight_id) REFERENCES flights(id) ON DELETE CASCADE;
ALTER TABLE ONLY reservations
    ADD CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;

CREATE ROLE kayttaja LOGIN PASSWORD 'salasana';

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE flights TO kayttaja;
GRANT SELECT,USAGE ON SEQUENCE flights_id_seq TO kayttaja;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE reservations TO kayttaja;
GRANT SELECT,USAGE ON SEQUENCE reservations_id_seq TO kayttaja;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE users TO kayttaja;
GRANT SELECT,USAGE ON SEQUENCE users_id_seq TO kayttaja;

