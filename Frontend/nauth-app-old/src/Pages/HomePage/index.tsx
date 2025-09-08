import { useContext, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer";
import { IUserProvider, UserContext } from "src/lib/nauth-core";

export default function HomePage() {

    const userContext = useContext<IUserProvider>(UserContext);

    let navigate = useNavigate();

    useEffect(() => {
        userContext.list(3).then((ret) => {
            if (!ret.sucesso) {
                alert(ret.mensagemErro);
            }
        });
    }, []);

    return (
        <>
            <Header />
            <Footer />
        </>
    );

}