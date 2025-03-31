import { BrowserRouter as Router } from "react-router-dom"
import NavMenuMain from "./components/nav"
import MainRoutes from "./routes"


const App : React.FC = () => {
    return(
        <Router>
            <NavMenuMain />
            <MainRoutes />
        </Router>
    )
}

export default App