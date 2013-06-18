--
-- PostgreSQL database dump
--

SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = off;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET escape_string_warning = off;

--
-- Name: attendance_batam; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE attendance_batam WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';


\connect attendance_batam

SET statement_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = off;
SET check_function_bodies = false;
SET client_min_messages = warning;
SET escape_string_warning = off;

--
-- Name: plpgsql; Type: PROCEDURAL LANGUAGE; Schema: -; Owner: -
--
-- Name: attendance_course; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE attendance_course (
    id character varying(32) NOT NULL,
    course_id character varying(10) NOT NULL,
    start_time timestamp(6) without time zone NOT NULL,
    device_id character varying(10) NOT NULL,
    status integer NOT NULL
);


--
-- Name: serial_number_mapping; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE serial_number_mapping (
    id character varying(32) NOT NULL,
    card_serial_number_decimal character varying DEFAULT 0,
    card_serial_number_hexa character varying DEFAULT 0
);


--
-- Name: student_attendance; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE student_attendance (
    id character varying(32) NOT NULL,
    card_serial_number character varying(16) NOT NULL,
    checkin_time timestamp without time zone NOT NULL,
    status integer NOT NULL,
    device_id character varying(10) NOT NULL,
    card_serial_number_hexa character varying
);


--
-- Data for Name: attendance_course; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A04', 'EL-3406', '2013-06-04 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A06', 'EL-3406', '2013-06-06 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A08', 'EL-3406', '2013-06-08 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A10', 'EL-3406', '2013-06-10 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A12', 'EL-3406', '2013-06-12 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A14', 'EL-3406', '2013-06-14 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A16', 'EL-3406', '2013-06-16 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A18', 'EL-3406', '2013-06-18 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A20', 'EL-3406', '2013-06-20 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A22', 'EL-3406', '2013-06-22 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A24', 'EL-3406', '2013-06-24 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A26', 'EL-3406', '2013-06-26 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A28', 'EL-3406', '2013-06-28 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A15A30', 'EL-3406', '2013-06-30 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A02', 'EL-3406', '2013-07-02 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A04', 'EL-3406', '2013-07-04 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A06', 'EL-3406', '2013-07-06 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A08', 'EL-3406', '2013-07-08 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A10', 'EL-3406', '2013-07-10 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A12', 'EL-3406', '2013-07-12 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A14', 'EL-3406', '2013-07-14 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A16', 'EL-3406', '2013-07-16 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A18', 'EL-3406', '2013-07-18 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A20', 'EL-3406', '2013-07-20 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A22', 'EL-3406', '2013-07-22 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A24', 'EL-3406', '2013-07-24 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A26', 'EL-3406', '2013-07-26 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A28', 'EL-3406', '2013-07-28 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A16A30', 'EL-3406', '2013-07-30 08:00:00', '2', 0);
INSERT INTO attendance_course (id, course_id, start_time, device_id, status) VALUES ('A17A02', 'EL-3406', '2013-08-02 08:00:00', '2', 0);

--
-- Name: attendance_course_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY attendance_course
    ADD CONSTRAINT attendance_course_pkey PRIMARY KEY (id);


--
-- Name: kehadiran_mahasiswa_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY student_attendance
    ADD CONSTRAINT kehadiran_mahasiswa_pkey PRIMARY KEY (id);


--
-- Name: serial_number_mapping_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY serial_number_mapping
    ADD CONSTRAINT serial_number_mapping_pkey PRIMARY KEY (id);


--
-- Name: insert_card_serial_number_hexa; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER insert_card_serial_number_hexa
    BEFORE INSERT OR UPDATE ON student_attendance
    FOR EACH ROW
    EXECUTE PROCEDURE insert_card_serial_number_hexa();


--
-- PostgreSQL database dump complete
--

