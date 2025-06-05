--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.1

-- Started on 2025-06-05 18:45:08

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--

CREATE SCHEMA public;


--
-- TOC entry 4469 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: -
--

COMMENT ON SCHEMA public IS 'standard public schema';


--
-- TOC entry 217 (class 1259 OID 16865)
-- Name: network_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.network_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 218 (class 1259 OID 16895)
-- Name: profile_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.profile_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 16916)
-- Name: user_addresses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_addresses (
    address_id bigint NOT NULL,
    user_id bigint NOT NULL,
    zip_code character varying(15),
    address character varying(150),
    complement character varying(150),
    neighborhood character varying(120),
    city character varying(120),
    state character varying(80)
);


--
-- TOC entry 220 (class 1259 OID 16921)
-- Name: user_addresses_address_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_addresses_address_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4470 (class 0 OID 0)
-- Dependencies: 220
-- Name: user_addresses_address_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_addresses_address_id_seq OWNED BY public.user_addresses.address_id;


--
-- TOC entry 221 (class 1259 OID 16922)
-- Name: user_documents; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_documents (
    document_id bigint NOT NULL,
    user_id bigint,
    document_type integer DEFAULT 0 NOT NULL,
    base64 text
);


--
-- TOC entry 222 (class 1259 OID 16928)
-- Name: user_documents_document_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_documents_document_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4471 (class 0 OID 0)
-- Dependencies: 222
-- Name: user_documents_document_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_documents_document_id_seq OWNED BY public.user_documents.document_id;


--
-- TOC entry 223 (class 1259 OID 16929)
-- Name: user_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 224 (class 1259 OID 16935)
-- Name: user_phones; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_phones (
    phone_id bigint NOT NULL,
    user_id bigint NOT NULL,
    phone character varying(30) NOT NULL
);


--
-- TOC entry 225 (class 1259 OID 16938)
-- Name: user_phones_phone_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_phones_phone_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4472 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_phones_phone_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_phones_phone_id_seq OWNED BY public.user_phones.phone_id;


--
-- TOC entry 226 (class 1259 OID 16945)
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    user_id bigint DEFAULT nextval('public.user_id_seq'::regclass) NOT NULL,
    created_at timestamp without time zone NOT NULL,
    updated_at timestamp without time zone NOT NULL,
    hash character varying(128),
    email character varying(180) NOT NULL,
    name character varying(120) NOT NULL,
    password character varying(128),
    is_admin boolean DEFAULT false NOT NULL,
    token character varying(128),
    recovery_hash character varying(128),
    id_document character varying(30),
    birth_date timestamp without time zone,
    pix_key character varying(180),
    slug character varying(140) NOT NULL,
    stripe_id character varying(120),
    image character varying(150)
);


--
-- TOC entry 4292 (class 2604 OID 16966)
-- Name: user_addresses address_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses ALTER COLUMN address_id SET DEFAULT nextval('public.user_addresses_address_id_seq'::regclass);


--
-- TOC entry 4293 (class 2604 OID 16967)
-- Name: user_documents document_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents ALTER COLUMN document_id SET DEFAULT nextval('public.user_documents_document_id_seq'::regclass);


--
-- TOC entry 4295 (class 2604 OID 16968)
-- Name: user_phones phone_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones ALTER COLUMN phone_id SET DEFAULT nextval('public.user_phones_phone_id_seq'::regclass);


--
-- TOC entry 4456 (class 0 OID 16916)
-- Dependencies: 219
-- Data for Name: user_addresses; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (14, 14, '73252900', 'Condomínio RK', 'Conj. Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (15, 16, '73252900', 'Condomínio RK', 'Conj Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (16, 17, '73252900', 'Condomínio RK', 'Qd N', 'Sobradinho', 'Brasília', 'DF');


--
-- TOC entry 4458 (class 0 OID 16922)
-- Dependencies: 221
-- Data for Name: user_documents; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 4461 (class 0 OID 16935)
-- Dependencies: 224
-- Data for Name: user_phones; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (16, 14, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (17, 16, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (18, 17, '61998752588');


--
-- TOC entry 4463 (class 0 OID 16945)
-- Dependencies: 226
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (16, '2025-04-17 18:13:11.378002', '2025-05-13 10:14:14.159478', 'wEjf5xwT5EI8BpRSdkwc43ECjDcRBc9Z8f4qdgoUaRaD_IsuWxOhagS_c8emvxRfXO_s42rROt0PMeBgzHlL4XfEn9fwqbyUB1Se', 'joao@gmail.com', 'Joao Pedro', '4CE829DB2AE704F65058C58E242A527C', false, 'D60FDD4734C01E858BB7445C78AA6E17', NULL, NULL, NULL, NULL, 'joao-pedro', NULL, 'user-1Zf7hayOxMOUeXSluI8pI3.jpg');
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (15, '2025-04-17 16:47:36.312183', '2025-04-17 16:47:36.312185', 'GA6CiRWIJZbbThZDWbixb9maJeg70cRgBrF1k1l0_ZCQYeiZqH2gP5Sn4kHDcZo6XdeKfZDYKUW5IXz-fMv26Viw9Zkgo9S0RJqi', 'vivianemelo@gmail.com', 'Viviane Melo', NULL, false, NULL, NULL, NULL, NULL, NULL, 'viviane-melo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (17, '2025-04-29 15:39:11.426121', '2025-04-29 15:39:12.1544', '7K5kI2X-ogDYyLp-gidZYJJ7-FwginPLKB_VmgHc6yGa6w1xL6cj74p55JCnbu60YuWKfiwIHW0JJDmNPGXssCilJMi4_2dtDoHT', 'joao.paulo@gmail.com', 'Joao Paulo', 'BA4EF91DAE03BC74B88FCCFD2E4DC060', false, 'B42D7FB37CF78E3749EC4E2ECC8120CA', NULL, NULL, NULL, NULL, 'joao-paulo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (14, '2025-04-17 16:04:48.360057', '2025-04-17 16:17:40.292963', 'pU8MILkTP95axC7-02EyMt2cA6fTNsSv-IK5z6KrbG7iS6n76c8VDVsq39pRVy319DorstlJ5Gy06WByFpqRTSEfmMmSWylIWXV2', 'landim32@gmail.com', 'Rodrigo Landim', NULL, false, 'tokendoroot', NULL, NULL, NULL, NULL, 'rodrigo-landim', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (18, '2025-05-16 19:29:13.049334', '2025-06-05 18:24:01.771807', 'r179_YN0ARX7Io9--SaKUop9SO8tYHdHjnUEPdk84-fdbSIFVewVUtur2mQiwdVYLMkq8tySHYynPav0b92_lVVwiZo0dQDIZVkY', 'rodrigo@emagine.com.br', 'Rodrigo Landim', '52A6BD009D45BCD900411191E42AA47C', false, 'B686A32A8437889F4F169537B3865FF4', NULL, NULL, NULL, NULL, 'rodrigo-landim1', NULL, 'user-1FhEorsT0qbGXcs3uiYHa6.jpg');


--
-- TOC entry 4473 (class 0 OID 0)
-- Dependencies: 217
-- Name: network_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.network_id_seq', 12, true);


--
-- TOC entry 4474 (class 0 OID 0)
-- Dependencies: 218
-- Name: profile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.profile_id_seq', 23, true);


--
-- TOC entry 4475 (class 0 OID 0)
-- Dependencies: 220
-- Name: user_addresses_address_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_addresses_address_id_seq', 16, true);


--
-- TOC entry 4476 (class 0 OID 0)
-- Dependencies: 222
-- Name: user_documents_document_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_documents_document_id_seq', 1, false);


--
-- TOC entry 4477 (class 0 OID 0)
-- Dependencies: 223
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_id_seq', 18, true);


--
-- TOC entry 4478 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_phones_phone_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_phones_phone_id_seq', 18, true);


--
-- TOC entry 4299 (class 2606 OID 16981)
-- Name: user_addresses pk_user_addresses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses
    ADD CONSTRAINT pk_user_addresses PRIMARY KEY (address_id);


--
-- TOC entry 4301 (class 2606 OID 16995)
-- Name: user_documents user_documents_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents
    ADD CONSTRAINT user_documents_pkey PRIMARY KEY (document_id);


--
-- TOC entry 4303 (class 2606 OID 16997)
-- Name: user_phones user_phones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones
    ADD CONSTRAINT user_phones_pkey PRIMARY KEY (phone_id);


--
-- TOC entry 4305 (class 2606 OID 17001)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4306 (class 2606 OID 17079)
-- Name: user_addresses fk_user_address; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses
    ADD CONSTRAINT fk_user_address FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4307 (class 2606 OID 17084)
-- Name: user_documents fk_user_document; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents
    ADD CONSTRAINT fk_user_document FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4308 (class 2606 OID 17109)
-- Name: user_phones fk_user_phone; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones
    ADD CONSTRAINT fk_user_phone FOREIGN KEY (user_id) REFERENCES public.users(user_id);


-- Completed on 2025-06-05 18:45:24

--
-- PostgreSQL database dump complete
--

