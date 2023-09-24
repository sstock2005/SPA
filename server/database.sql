SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";

CREATE TABLE `spa` (
  `id` int NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `passwords` (
  `id` int NOT NULL,
  `owner` varchar(30) NOT NULL,
  `url` varchar(30) NOT NULL,
  `useremail` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `spa`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `passwords`
  ADD PRIMARY KEY (`id`);

ALTER TABLE `spa`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

ALTER TABLE `passwords`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=112;
COMMIT;