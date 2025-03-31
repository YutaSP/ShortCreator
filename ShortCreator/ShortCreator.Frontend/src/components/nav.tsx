import { Link } from 'react-router-dom'

const NavMenuMain : React.FC = () => {
    return(
        <nav>
            <ul>
                <li>
                    <Link to="/">Home</Link>
                </li>
                <li>
                    <Link to="/management">Manage</Link>
                </li>
            </ul>
        </nav>
    )
}

export default NavMenuMain