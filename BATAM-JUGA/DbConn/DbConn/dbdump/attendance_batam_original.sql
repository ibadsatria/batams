--
-- PostgreSQL database dump
--

-- Dumped from database version 8.4.8
-- Dumped by pg_dump version 9.1.7
-- Started on 2013-03-17 08:07:09


CREATE DATABASE attendance_batam WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';


CREATE TABLE attendance_course (
    id character varying(32) NOT NULL,
    course_id character varying(10) NOT NULL,
    start_time timestamp without time zone NOT NULL,
    device_id character varying(10) NOT NULL,
    status integer NOT NULL
);

--- KETERANGAN table attendance_course
-- id: id unik table attendance_course
-- course_id: kode mata kuliah yang diinput oleh dosen/ work code
-- start_time: waktu memulai mata kuliah/ timestamp dosen menginput workcode
-- device_id: id device dari mana data ini berasal
-- status: status apakah data sudah di-pull oleh sistem back end atau belum. Diisi dengan angka 0 saja

CREATE TABLE student_attendance (
    id character varying(32) NOT NULL,
    card_serial_number character varying(16) NOT NULL,
    checkin_time timestamp without time zone NOT NULL,
    status integer NOT NULL,
    device_id character varying(10) NOT NULL
);

--- KETERANGAN table student_attendance
-- id: id unik untuk table student_attendance
-- card_serial_number: CSN untuk kartu mahasiswa
-- checkin_time: waktu check in/tap kartu mahasiswa
-- status: status apakah data sudah di-pull oleh sistem back end atau belum. Diisi dengan angka 0 saja
-- device_id: id device dari mana data ini berasal


-- Completed on 2013-03-17 08:07:28

--
-- PostgreSQL database dump complete
--

