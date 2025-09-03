--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.1

-- Started on 2025-09-03 17:23:47

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
-- TOC entry 4465 (class 0 OID 16916)
-- Dependencies: 219
-- Data for Name: user_addresses; Type: TABLE DATA; Schema: public; Owner: doadmin
--

INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (14, 14, '73252900', 'Condomínio RK', 'Conj. Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (15, 16, '73252900', 'Condomínio RK', 'Conj Centauros Qd N Casa 42', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (16, 17, '73252900', 'Condomínio RK', 'Qd N', 'Sobradinho', 'Brasília', 'DF');
INSERT INTO public.user_addresses (address_id, user_id, zip_code, address, complement, neighborhood, city, state) VALUES (17, 19, '73252900', 'Condomínio RK', 'Conj Centauros Qd N', 'Sobradinho', 'Brasília', 'DF');


--
-- TOC entry 4467 (class 0 OID 16922)
-- Dependencies: 221
-- Data for Name: user_documents; Type: TABLE DATA; Schema: public; Owner: doadmin
--



--
-- TOC entry 4470 (class 0 OID 16935)
-- Dependencies: 224
-- Data for Name: user_phones; Type: TABLE DATA; Schema: public; Owner: doadmin
--

INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (16, 14, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (17, 16, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (18, 17, '61998752588');
INSERT INTO public.user_phones (phone_id, user_id, phone) VALUES (19, 19, '61998752588');


--
-- TOC entry 4474 (class 0 OID 17397)
-- Dependencies: 228
-- Data for Name: user_tokens; Type: TABLE DATA; Schema: public; Owner: doadmin
--

INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (1, 18, '2025-08-26 18:05:03.694356', '2025-08-26 18:05:03.694356', '2025-10-26 18:05:03.694356', '123456', '::1', '247KbvXcebysVHl4AndmpS', 'PostmanRuntime/7.44.1');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (2, 18, '2025-08-26 18:12:42.282214', '2025-08-26 18:12:42.282214', '2025-10-26 18:12:42.282214', '123456', '::1', '3D6WHRF5FSknxUzjw2u91q', 'PostmanRuntime/7.44.1');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (3, 18, '2025-08-26 23:56:16.911086', '2025-08-26 23:56:16.911086', '2025-10-26 23:56:16.911086', '123456', '::ffff:172.18.0.4', '1iw72yDUaQ3EKXiql0YbFY', 'PostmanRuntime/7.45.0');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (4, 18, '2025-08-28 14:16:14.470528', '2025-08-28 14:16:14.470528', '2025-10-28 14:16:14.470528', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', 'CqhVJBtvGJfVGVEJi3uoE', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');
INSERT INTO public.user_tokens (token_id, user_id, created_at, last_access, expire_at, fingerprint, ip_address, token, user_agent) VALUES (5, 18, '2025-08-28 15:02:30.909325', '2025-08-28 15:02:30.909325', '2025-10-28 15:02:30.909325', '0bf3f27e390ad801ecf660b84467969d', '::ffff:172.18.0.4', '3FDYi2Ms6SEceES0aAFiSu', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36');


--
-- TOC entry 4472 (class 0 OID 16945)
-- Dependencies: 226
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: doadmin
--

INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (16, '2025-04-17 18:13:11.378002', '2025-05-13 10:14:14.159478', 'wEjf5xwT5EI8BpRSdkwc43ECjDcRBc9Z8f4qdgoUaRaD_IsuWxOhagS_c8emvxRfXO_s42rROt0PMeBgzHlL4XfEn9fwqbyUB1Se', 'joao@gmail.com', 'Joao Pedro', '4CE829DB2AE704F65058C58E242A527C', false, 'D60FDD4734C01E858BB7445C78AA6E17', NULL, NULL, NULL, NULL, 'joao-pedro', NULL, 'user-1Zf7hayOxMOUeXSluI8pI3.jpg');
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (15, '2025-04-17 16:47:36.312183', '2025-04-17 16:47:36.312185', 'GA6CiRWIJZbbThZDWbixb9maJeg70cRgBrF1k1l0_ZCQYeiZqH2gP5Sn4kHDcZo6XdeKfZDYKUW5IXz-fMv26Viw9Zkgo9S0RJqi', 'vivianemelo@gmail.com', 'Viviane Melo', NULL, false, NULL, NULL, NULL, NULL, NULL, 'viviane-melo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (17, '2025-04-29 15:39:11.426121', '2025-04-29 15:39:12.1544', '7K5kI2X-ogDYyLp-gidZYJJ7-FwginPLKB_VmgHc6yGa6w1xL6cj74p55JCnbu60YuWKfiwIHW0JJDmNPGXssCilJMi4_2dtDoHT', 'joao.paulo@gmail.com', 'Joao Paulo', 'BA4EF91DAE03BC74B88FCCFD2E4DC060', false, 'B42D7FB37CF78E3749EC4E2ECC8120CA', NULL, NULL, NULL, NULL, 'joao-paulo', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (14, '2025-04-17 16:04:48.360057', '2025-04-17 16:17:40.292963', 'pU8MILkTP95axC7-02EyMt2cA6fTNsSv-IK5z6KrbG7iS6n76c8VDVsq39pRVy319DorstlJ5Gy06WByFpqRTSEfmMmSWylIWXV2', 'landim32@gmail.com', 'Rodrigo Landim', NULL, false, 'tokendoroot', NULL, NULL, NULL, NULL, 'rodrigo-landim', NULL, NULL);
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (18, '2025-05-16 19:29:13.049334', '2025-06-05 18:24:01.771807', 'r179_YN0ARX7Io9--SaKUop9SO8tYHdHjnUEPdk84-fdbSIFVewVUtur2mQiwdVYLMkq8tySHYynPav0b92_lVVwiZo0dQDIZVkY', 'rodrigo@emagine.com.br', 'Rodrigo Landim', '52A6BD009D45BCD900411191E42AA47C', false, '653D4C028462969BCFA11CC2A71CD7E1', NULL, NULL, NULL, NULL, 'rodrigo-landim1', NULL, 'user-1FhEorsT0qbGXcs3uiYHa6.jpg');
INSERT INTO public.users (user_id, created_at, updated_at, hash, email, name, password, is_admin, token, recovery_hash, id_document, birth_date, pix_key, slug, stripe_id, image) VALUES (19, '2025-08-26 20:51:28.284665', '2025-08-26 20:51:28.995512', 'hwRhH6LyuGKFZhp2JPjO89xxbTLDHUndegbh5PytfyxVyFI2cTSB_8xxlLn5OgqVVN-tvXQ8oNALOpuQwmpRrUSEL4a6LviPjYYV', 'rodrigo.landim@emagine.com.br', 'Rodrigo Landim', '7DD0DFFD8A579E88169CD17F757DB29A', false, NULL, NULL, NULL, NULL, NULL, 'rodrigo-landim2', NULL, NULL);


--
-- TOC entry 4485 (class 0 OID 0)
-- Dependencies: 217
-- Name: network_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.network_id_seq', 12, true);


--
-- TOC entry 4486 (class 0 OID 0)
-- Dependencies: 218
-- Name: profile_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.profile_id_seq', 23, true);


--
-- TOC entry 4487 (class 0 OID 0)
-- Dependencies: 220
-- Name: user_addresses_address_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.user_addresses_address_id_seq', 17, true);


--
-- TOC entry 4488 (class 0 OID 0)
-- Dependencies: 222
-- Name: user_documents_document_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.user_documents_document_id_seq', 1, false);


--
-- TOC entry 4489 (class 0 OID 0)
-- Dependencies: 223
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.user_id_seq', 19, true);


--
-- TOC entry 4490 (class 0 OID 0)
-- Dependencies: 225
-- Name: user_phones_phone_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.user_phones_phone_id_seq', 19, true);


--
-- TOC entry 4491 (class 0 OID 0)
-- Dependencies: 227
-- Name: user_tokens_token_id_seq; Type: SEQUENCE SET; Schema: public; Owner: doadmin
--

SELECT pg_catalog.setval('public.user_tokens_token_id_seq', 5, true);


-- Completed on 2025-09-03 17:24:04

--
-- PostgreSQL database dump complete
--

