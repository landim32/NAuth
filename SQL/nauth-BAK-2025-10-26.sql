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
-- TOC entry 4480 (class 0 OID 0)
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
-- TOC entry 4481 (class 0 OID 0)
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
-- TOC entry 4482 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_phones_phone_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_phones_phone_id_seq OWNED BY public.user_phones.phone_id;


--
-- TOC entry 228 (class 1259 OID 17397)
-- Name: user_tokens; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_tokens (
    token_id bigint NOT NULL,
    user_id bigint NOT NULL,
    created_at timestamp without time zone NOT NULL,
    last_access timestamp without time zone NOT NULL,
    expire_at timestamp without time zone NOT NULL,
    fingerprint character varying(40) NOT NULL,
    ip_address character varying(64) NOT NULL,
    token character varying(128) NOT NULL,
    user_agent character varying(512) NOT NULL
);


--
-- TOC entry 227 (class 1259 OID 17396)
-- Name: user_tokens_token_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.user_tokens_token_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4483 (class 0 OID 0)
-- Dependencies: 227
-- Name: user_tokens_token_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.user_tokens_token_id_seq OWNED BY public.user_tokens.token_id;


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
-- TOC entry 4297 (class 2604 OID 16966)
-- Name: user_addresses address_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses ALTER COLUMN address_id SET DEFAULT nextval('public.user_addresses_address_id_seq'::regclass);


--
-- TOC entry 4298 (class 2604 OID 16967)
-- Name: user_documents document_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents ALTER COLUMN document_id SET DEFAULT nextval('public.user_documents_document_id_seq'::regclass);


--
-- TOC entry 4300 (class 2604 OID 16968)
-- Name: user_phones phone_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones ALTER COLUMN phone_id SET DEFAULT nextval('public.user_phones_phone_id_seq'::regclass);


--
-- TOC entry 4303 (class 2604 OID 17400)
-- Name: user_tokens token_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_tokens ALTER COLUMN token_id SET DEFAULT nextval('public.user_tokens_token_id_seq'::regclass);


--
-- TOC entry 4465 (class 0 OID 16916)
-- Dependencies: 219
-- Data for Name: user_addresses; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (14, 14, '73252900', 'Condomínio RK', 'Conj. Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (15, 16, '73252900', 'Condomínio RK', 'Conj Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (16, 17, '73252900', 'Condomínio RK', 'Qd N', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (17, 19, '73252900', 'Condomínio RK', 'Conj Centauros Qd N', 'Sobradinho', 'Brasília', 'DF');


--
-- TOC entry 4467 (class 0 OID 16922)
-- Dependencies: 221
-- Data for Name: user_documents; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- TOC entry 4470 (class 0 OID 16935)
-- Dependencies: 224
-- Data for Name: user_phones; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (16, 14, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (17, 16, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (18, 17, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (19, 19, '61998752588');


--
-- TOC entry 4474 (class 0 OID 17397)
-- Dependencies: 228
-- Data for Name: user_tokens; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (1, 18, '2025-08-26 18:05:03.694356', '2025-08-26 18:05:03.694356', '2025-10-26 18:05:03.694356', '123456', '::1', '247KbvXcebysVHl4AndmpS', 'PostmanRuntime/7.44.1');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (2, 18, '2025-08-26 18:12:42.282214', '2025-08-26 18:12:42.282214', '2025-10-26 18:12:42.282214', '123456', '::1', '3D6WHRF5FSknxUzjw2u91q', 'PostmanRuntime/7.44.1');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (3, 18, '2025-08-26 23:56:16.911086', '2025-08-26 23:56:16.911086', '2025-10-26 23:56:16.911086', '123456', '::ffff:172.18.0.4', '1iw72yDUaQ3EKXiql0YbFY', 'PostmanRuntime/7.45.0');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (4, 18, '2025-08-28 14:16:14.470528', '2025-08-28 14:16:14.470528', '2025-10-28 14:16:14.470528', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', 'CqhVJBtvGJfVGVEJi3uoE', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (5, 18, '2025-08-28 15:02:30.909325', '2025-08-28 15:02:30.909325', '2025-10-28 15:02:30.909325', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '3FDYi2Ms6SEceES0aAFiSu', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (6, 18, '2025-09-04 15:28:28.027428', '2025-09-04 15:28:28.027428', '2025-11-04 15:28:28.027428', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '1c3WIl2MRLqpCk4NCIBVdL', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (7, 18, '2025-09-04 15:28:38.988347', '2025-09-04 15:28:38.988347', '2025-11-04 15:28:38.988347', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '1oXhNJNTa9ffE3DAwtuBEb', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (8, 18, '2025-09-04 16:10:56.155807', '2025-09-04 16:10:56.155807', '2025-11-04 16:10:56.155807', '497651481af35797bf46810c960ea926', '::1', 'T9VW7jhpQJfY9wXMylOn4', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (9, 18, '2025-09-04 21:06:51.442371', '2025-09-04 21:06:51.442371', '2025-11-04 21:06:51.442371', '0bf3f27e390ad801ecf660b84467969d', '::1', '2zTu5qKmXkKXziMLTZ4hLy', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (42, 18, '2025-09-04 21:35:43.259493', '2025-09-04 21:35:43.259493', '2025-11-04 21:35:43.259493', '0bf3f27e390ad801ecf660b84467969d', '::1', '34ni0b3ThDkTwKwRMB0MuR', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (43, 18, '2025-09-04 21:36:17.126963', '2025-09-04 21:36:17.126963', '2025-11-04 21:36:17.126963', '0bf3f27e390ad801ecf660b84467969d', '::1', '1mg9WmoD1lhRVB7ZzG4RI7', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (44, 18, '2025-09-04 21:37:27.778534', '2025-09-04 21:37:27.778534', '2025-11-04 21:37:27.778534', '0bf3f27e390ad801ecf660b84467969d', '::1', 'u4r6HQ0bN9TGt6FqItdje', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (45, 18, '2025-09-07 09:35:30.453351', '2025-09-07 09:35:30.453351', '2025-11-07 09:35:30.453351', '0bf3f27e390ad801ecf660b84467969d', '::1', '1Q7WxiEbRECrVEBR2YEKSx', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (46, 18, '2025-09-07 10:46:48.864017', '2025-09-07 10:46:48.864017', '2025-11-07 10:46:48.864017', '0bf3f27e390ad801ecf660b84467969d', '::1', '2gbkNEZUIKvmit18MNS0dy', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (47, 18, '2025-09-07 10:57:07.518727', '2025-09-07 10:57:07.518727', '2025-11-07 10:57:07.518727', '0bf3f27e390ad801ecf660b84467969d', '::1', '2O7ESSVW18T035Lk21dyW1', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (48, 18, '2025-09-07 10:57:17.530942', '2025-09-07 10:57:17.530942', '2025-11-07 10:57:17.530942', '0bf3f27e390ad801ecf660b84467969d', '::1', '13vxdWyc4IRGbSITO7e9CT', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (49, 18, '2025-09-07 14:56:53.138939', '2025-09-07 14:56:53.138939', '2025-11-07 14:56:53.138939', '0bf3f27e390ad801ecf660b84467969d', '::1', 'MqRxf5V64VGUiyFibRcCf', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (50, 18, '2025-09-07 15:55:18.536799', '2025-09-07 15:55:18.536799', '2025-11-07 15:55:18.536799', '0bf3f27e390ad801ecf660b84467969d', '::1', 'pcEH73hewUmo6biXb43sq', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (51, 18, '2025-09-07 16:43:11.394137', '2025-09-07 16:43:11.394137', '2025-11-07 16:43:11.394137', '0bf3f27e390ad801ecf660b84467969d', '::1', '31VwURGTh5Cwl0YzCkaa9Y', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (52, 18, '2025-09-08 11:50:01.322409', '2025-09-08 11:50:01.322409', '2025-11-08 11:50:01.322409', '123456', '::1', '2j7fs0fOVZpTnQKSv9tdfY', 'PostmanRuntime/7.45.0');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (53, 18, '2025-09-08 14:58:01.53663', '2025-09-08 14:58:01.53663', '2025-11-08 14:58:01.53663', '123456', '::ffff:172.18.0.4', '2q1zxU9iUfrIicI6RWuhYp', 'PostmanRuntime/7.45.0');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (54, 18, '2025-09-08 15:04:44.213798', '2025-09-08 15:04:44.213798', '2025-11-08 15:04:44.213798', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '1SQndRhL72ZT8peUOc0pmN', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (55, 18, '2025-09-08 17:30:46.482502', '2025-09-08 17:30:46.482502', '2025-11-08 17:30:46.482502', '497651481af35797bf46810c960ea926', '::ffff:172.18.0.4', 'Sc0q8TGySpBzOTSSo7ERP', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (56, 18, '2025-09-08 20:16:52.911181', '2025-09-08 20:16:52.911181', '2025-11-08 20:16:52.911181', '497651481af35797bf46810c960ea926', '::ffff:172.18.0.4', '2J01pcSWvUVatta5mfGql9', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (57, 18, '2025-09-08 20:30:17.09647', '2025-09-08 20:30:17.09647', '2025-11-08 20:30:17.09647', '497651481af35797bf46810c960ea926', '::ffff:172.18.0.4', '2PYKqXdMjd31BG4CV5fyAj', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (58, 18, '2025-09-08 20:56:30.854469', '2025-09-08 20:56:30.854469', '2025-11-08 20:56:30.854469', '497651481af35797bf46810c960ea926', '::ffff:172.18.0.4', '2QkWCXj452bKDrcPYY46xu', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (59, 18, '2025-09-09 00:22:51.882953', '2025-09-09 00:22:51.882953', '2025-11-09 00:22:51.882953', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '2S3CnRLSBpNC5Wn78Vrm1a', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (60, 18, '2025-09-12 01:05:25.117462', '2025-09-12 01:05:25.117462', '2025-11-12 01:05:25.117462', 'af9f4c8b46a327ce342af106a0835d4e', '::ffff:172.18.0.4', '32QOGvaADP9Rh1fl8axLn0', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36');


--
-- TOC entry 4472 (class 0 OID 16945)
-- Dependencies: 226
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (16, '2025-04-17 18:13:11.378002', '2025-05-13 10:14:14.159478', 'wEjf5xwT5EI8BpRSdkwc43ECjDcRBc9Z8f4qdgoUaRaD_IsuWxOhagS_c8emvxRfXO_s42rROt0PMeBgzHlL4XfEn9fwqbyUB1Se', 'joao@gmail.com', 'Joao Pedro', '4CE829DB2AE704F65058C58E242A527C', false, 'D60FDD4734C01E858BB7445C78AA6E17', NULL, NULL, NULL, NULL, 'joao-pedro', NULL, 'user-1Zf7hayOxMOUeXSluI8pI3.jpg');
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (15, '2025-04-17 16:47:36.312183', '2025-04-17 16:47:36.312185', 'GA6CiRWIJZbbThZDWbixb9maJeg70cRgBrF1k1l0_ZCQYeiZqH2gP5Sn4kHDcZo6XdeKfZDYKUW5IXz-fMv26Viw9Zkgo9S0RJqi', 'vivianemelo@gmail.com', 'Viviane Melo', NULL, false, NULL, NULL, NULL, NULL, NULL, 'viviane-melo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (17, '2025-04-29 15:39:11.426121', '2025-04-29 15:39:12.1544', '7K5kI2X-ogDYyLp-gidZYJJ7-FwginPLKB_VmgHc6yGa6w1xL6cj74p55JCnbu60YuWKfiwIHW0JJDmNPGXssCilJMi4_2dtDoHT', 'joao.paulo@gmail.com', 'Joao Paulo', 'BA4EF91DAE03BC74B88FCCFD2E4DC060', false, 'B42D7FB37CF78E3749EC4E2ECC8120CA', NULL, NULL, NULL, NULL, 'joao-paulo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (14, '2025-04-17 16:04:48.360057', '2025-04-17 16:17:40.292963', 'pU8MILkTP95axC7-02EyMt2cA6fTNsSv-IK5z6KrbG7iS6n76c8VDVsq39pRVy319DorstlJ5Gy06WByFpqRTSEfmMmSWylIWXV2', 'landim32@gmail.com', 'Rodrigo Landim', NULL, false, 'tokendoroot', NULL, NULL, NULL, NULL, 'rodrigo-landim', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (18, '2025-05-16 19:29:13.049334', '2025-06-05 18:24:01.771807', 'r179_YN0ARX7Io9--SaKUop9SO8tYHdHjnUEPdk84-fdbSIFVewVUtur2mQiwdVYLMkq8tySHYynPav0b92_lVVwiZo0dQDIZVkY', 'rodrigo@emagine.com.br', 'Rodrigo Landim', '52A6BD009D45BCD900411191E42AA47C', false, '653D4C028462969BCFA11CC2A71CD7E1', NULL, NULL, NULL, NULL, 'rodrigo-landim1', NULL, 'user-1FhEorsT0qbGXcs3uiYHa6.jpg');
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (19, '2025-08-26 20:51:28.284665', '2025-08-26 20:51:28.995512', 'hwRhH6LyuGKFZhp2JPjO89xxbTLDHUndegbh5PytfyxVyFI2cTSB_8xxlLn5OgqVVN-tvXQ8oNALOpuQwmpRrUSEL4a6LviPjYYV', 'rodrigo.landim@emagine.com.br', 'Rodrigo Landim', '7DD0DFFD8A579E88169CD17F757DB29A', false, NULL, NULL, NULL, NULL, NULL, 'rodrigo-landim2', NULL, NULL);


--
-- TOC entry 4484 (class 0 OID 0)
-- Dependencies: 217
-- Name: network_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.network_id_seq', 12, true);


--
-- TOC entry 4485 (class 0 OID 0)
-- Dependencies: 218
-- Name: profile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.profile_id_seq', 23, true);


--
-- TOC entry 4486 (class 0 OID 0)
-- Dependencies: 220
-- Name: user_addresses_address_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_addresses_address_id_seq', 17, true);


--
-- TOC entry 4487 (class 0 OID 0)
-- Dependencies: 222
-- Name: user_documents_document_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_documents_document_id_seq', 1, false);


--
-- TOC entry 4488 (class 0 OID 0)
-- Dependencies: 223
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_id_seq', 19, true);


--
-- TOC entry 4489 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_phones_phone_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_phones_phone_id_seq', 19, true);


--
-- TOC entry 4490 (class 0 OID 0)
-- Dependencies: 227
-- Name: user_tokens_token_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.user_tokens_token_id_seq', 60, true);


--
-- TOC entry 4305 (class 2606 OID 16981)
-- Name: user_addresses pk_user_addresses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses
    ADD CONSTRAINT pk_user_addresses PRIMARY KEY (address_id);


--
-- TOC entry 4307 (class 2606 OID 16995)
-- Name: user_documents user_documents_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents
    ADD CONSTRAINT user_documents_pkey PRIMARY KEY (document_id);


--
-- TOC entry 4309 (class 2606 OID 16997)
-- Name: user_phones user_phones_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones
    ADD CONSTRAINT user_phones_pkey PRIMARY KEY (phone_id);


--
-- TOC entry 4313 (class 2606 OID 17404)
-- Name: user_tokens user_tokens_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_tokens
    ADD CONSTRAINT user_tokens_pkey PRIMARY KEY (token_id);


--
-- TOC entry 4311 (class 2606 OID 17001)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4317 (class 2606 OID 17405)
-- Name: user_tokens fk_token_user; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_tokens
    ADD CONSTRAINT fk_token_user FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4314 (class 2606 OID 17079)
-- Name: user_addresses fk_user_address; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_addresses
    ADD CONSTRAINT fk_user_address FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4315 (class 2606 OID 17084)
-- Name: user_documents fk_user_document; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_documents
    ADD CONSTRAINT fk_user_document FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4316 (class 2606 OID 17109)
-- Name: user_phones fk_user_phone; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_phones
    ADD CONSTRAINT fk_user_phone FOREIGN KEY (user_id) REFERENCES public.users(user_id);

