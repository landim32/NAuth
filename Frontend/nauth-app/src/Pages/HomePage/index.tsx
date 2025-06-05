import { useContext, useEffect, useState } from "react";
import AuthContext from "../../NAuth/Contexts/Auth/AuthContext";
import { Link, useNavigate } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer";
import UserContext from "../../NAuth/Contexts/User/UserContext";



export default function HomePage() {

    const authContext = useContext(AuthContext);
    const userContext = useContext(UserContext);

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